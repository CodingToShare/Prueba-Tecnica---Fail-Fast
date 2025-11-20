using Erp.Documents.Application.DTOs;
using Erp.Documents.Application.Interfaces;
using Erp.Documents.Domain;
using Erp.Documents.Domain.Entities;
using Erp.Documents.Domain.Enums;
using Erp.Documents.Infrastructure.Configuration;
using Erp.Documents.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Erp.Documents.Infrastructure.Services
{
    /// <summary>
    /// Implementación de IUploadDocumentService.
    /// Orquesta creación de documento, generación de URL presignada y validación de almacenamiento.
    /// </summary>
    public class UploadDocumentService : IUploadDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IValidationFlowRepository _validationFlowRepository;
        private readonly IObjectStorageService _storageService;
        private readonly ApiOptions _apiOptions;
        private readonly ValidationOptions _validationOptions;
        private readonly ILogger<UploadDocumentService> _logger;

        public UploadDocumentService(
            IDocumentRepository documentRepository,
            IValidationFlowRepository validationFlowRepository,
            IObjectStorageService storageService,
            IOptions<ApiOptions> apiOptions,
            IOptions<ValidationOptions> validationOptions,
            ILogger<UploadDocumentService> logger)
        {
            _documentRepository = documentRepository;
            _validationFlowRepository = validationFlowRepository;
            _storageService = storageService;
            _apiOptions = apiOptions.Value;
            _validationOptions = validationOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Inicia carga: crea documento, genera URL presignada.
        /// </summary>
        public async Task<UploadDocumentResponse> InitiateUploadAsync(UploadDocumentRequest request)
        {
            try
            {
                // Validación de solicitud
                if (request.CompanyId == Guid.Empty)
                    throw new ArgumentException("CompanyId es requerido");

                if (string.IsNullOrWhiteSpace(request.EntityType) || string.IsNullOrWhiteSpace(request.EntityId))
                    throw new ArgumentException("EntityType y EntityId son requeridos");

                if (string.IsNullOrWhiteSpace(request.FileName))
                    throw new ArgumentException("FileName es requerido");

                if (request.FileSizeBytes <= 0)
                    throw new ArgumentException("FileSizeBytes debe ser mayor a 0");

                // Validar tamaño máximo
                if (request.FileSizeBytes > _validationOptions.MaxFileSizeBytes)
                    throw new InvalidOperationException(
                        $"Archivo excede tamaño máximo permitido ({_validationOptions.MaxFileSizeBytes} bytes)");

                // Validar MIME type
                if (!_validationOptions.AllowedMimeTypes.Contains(request.MimeType))
                    throw new InvalidOperationException(
                        $"MIME type '{request.MimeType}' no está permitido");

                // Generar clave única del bucket
                var bucketKey = GenerateBucketKey(request.CompanyId, request.EntityType, request.EntityId, request.FileName);

                // Crear documento en base de datos
                var document = new Document
                {
                    Id = Guid.NewGuid(),
                    CompanyId = request.CompanyId,
                    EntityType = request.EntityType,
                    EntityId = request.EntityId,
                    Name = request.FileName,
                    MimeType = request.MimeType,
                    SizeBytes = request.FileSizeBytes,
                    BucketKey = bucketKey,
                    CreatedByUserId = request.UploadedByUserId ?? "system",
                    ValidationStatus = request.RequiresValidation ? ValidationStatus.P : null, // P = Pending
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };

                // Si requiere validación, crear flujo de validación
                if (request.RequiresValidation)
                {
                    var validationFlow = new DocumentValidationFlow
                    {
                        Id = Guid.NewGuid(),
                        DocumentId = document.Id,
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow,
                        Steps = new List<ValidationStep>(),
                        Actions = new List<ValidationAction>()
                    };

                    // Crear primer paso de validación (ejemplo: Order 1)
                    var firstStep = new ValidationStep
                    {
                        Id = Guid.NewGuid(),
                        FlowId = validationFlow.Id,
                        Order = 1,
                        Status = StepApprovalStatus.Pending,
                        CreatedAtUtc = DateTime.UtcNow,
                        UpdatedAtUtc = DateTime.UtcNow
                    };

                    validationFlow.Steps.Add(firstStep);
                    document.ValidationFlowId = validationFlow.Id;

                    await _validationFlowRepository.CreateAsync(validationFlow);
                }

                // Guardar documento
                await _documentRepository.CreateAsync(document);

                // Generar URL presignada para carga
                var presignedUrl = await _storageService.GeneratePresignedUploadUrlAsync(
                    bucketKey: bucketKey,
                    mimeType: request.MimeType,
                    sizeBytes: request.FileSizeBytes,
                    expiresInMinutes: _apiOptions.PresignedUrlExpirationMinutes
                );

                _logger.LogInformation(
                    "Carga de documento iniciada. DocumentId: {DocumentId}, CompanyId: {CompanyId}, BucketKey: {BucketKey}",
                    document.Id,
                    request.CompanyId,
                    bucketKey);

                return new UploadDocumentResponse
                {
                    DocumentId = document.Id,
                    PresignedUploadUrl = presignedUrl,
                    BucketKey = bucketKey,
                    ExpiresInMinutes = _apiOptions.PresignedUrlExpirationMinutes,
                    Status = request.RequiresValidation ? "Pending" : "Uploaded"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error iniciando carga de documento");
                throw;
            }
        }

        /// <summary>
        /// Completa carga: verifica existencia en storage y actualiza estado.
        /// </summary>
        public async Task<UploadDocumentResponse> CompleteUploadAsync(Guid documentId)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(documentId);
                if (document == null)
                    throw new FileNotFoundException($"Documento {documentId} no encontrado");

                // Verificar que archivo existe en storage
                var exists = await _storageService.ObjectExistsAsync(document.BucketKey);
                if (!exists)
                    throw new InvalidOperationException(
                        $"Archivo no encontrado en storage. BucketKey: {document.BucketKey}");

                // Obtener metadatos
                var (_, sizeBytes) = await _storageService.GetObjectMetadataAsync(document.BucketKey);

                // Actualizar documento
                document.UpdatedAtUtc = DateTime.UtcNow;
                await _documentRepository.UpdateAsync(document);

                // Obtener presigned URL para descarga
                var downloadUrl = await _storageService.GeneratePresignedDownloadUrlAsync(
                    bucketKey: document.BucketKey,
                    expiresInMinutes: _apiOptions.PresignedUrlExpirationMinutes
                );

                _logger.LogInformation(
                    "Carga de documento completada. DocumentId: {DocumentId}",
                    documentId);

                return new UploadDocumentResponse
                {
                    DocumentId = document.Id,
                    PresignedUploadUrl = downloadUrl,
                    BucketKey = document.BucketKey,
                    ExpiresInMinutes = _apiOptions.PresignedUrlExpirationMinutes,
                    Status = document.ValidationStatus.HasValue ? "PendingValidation" : "Completed"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completando carga de documento: {DocumentId}", documentId);
                throw;
            }
        }

        /// <summary>
        /// Genera clave única del bucket: "documents/company-{id}/entitytype/entityid-{filename}"
        /// </summary>
        private static string GenerateBucketKey(
            Guid companyId,
            string entityType,
            string entityId,
            string fileName)
        {
            // Sanitizar nombre de archivo
            var sanitizedFileName = Path.GetFileNameWithoutExtension(fileName)
                .Replace(" ", "-")
                .ToLowerInvariant();
            var extension = Path.GetExtension(fileName);

            return $"documents/company-{companyId:N}/{entityType.ToLowerInvariant()}/{entityId}-{sanitizedFileName}{extension}";
        }
    }
}
