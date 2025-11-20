using Erp.Documents.Application.DTOs;
using Erp.Documents.Application.Interfaces;
using Erp.Documents.Infrastructure.Configuration;
using Erp.Documents.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Erp.Documents.Infrastructure.Services
{
    /// <summary>
    /// Implementación de IDownloadDocumentService.
    /// Genera URLs presignadas para acceso seguro a archivos.
    /// </summary>
    public class DownloadDocumentService : IDownloadDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IObjectStorageService _storageService;
        private readonly ApiOptions _apiOptions;
        private readonly ILogger<DownloadDocumentService> _logger;

        public DownloadDocumentService(
            IDocumentRepository documentRepository,
            IObjectStorageService storageService,
            IOptions<ApiOptions> apiOptions,
            ILogger<DownloadDocumentService> logger)
        {
            _documentRepository = documentRepository;
            _storageService = storageService;
            _apiOptions = apiOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Genera URL presignada para descargar un documento.
        /// </summary>
        public async Task<DownloadDocumentResponse> GetDownloadUrlAsync(Guid documentId)
        {
            try
            {
                // Obtener documento
                var document = await _documentRepository.GetByIdAsync(documentId);
                if (document == null)
                    throw new FileNotFoundException($"Documento {documentId} no encontrado");

                // Verificar que archivo existe en storage
                var (exists, sizeBytes) = await _storageService.GetObjectMetadataAsync(document.BucketKey);
                if (!exists)
                    throw new InvalidOperationException(
                        $"Archivo no encontrado en storage. BucketKey: {document.BucketKey}");

                // Generar URL presignada de descarga
                var downloadUrl = await _storageService.GeneratePresignedDownloadUrlAsync(
                    bucketKey: document.BucketKey,
                    expiresInMinutes: _apiOptions.PresignedUrlExpirationMinutes
                );

                // Extraer nombre del archivo desde bucketKey
                var fileName = Path.GetFileName(document.BucketKey);

                _logger.LogInformation(
                    "URL de descarga generada. DocumentId: {DocumentId}",
                    documentId);

                return new DownloadDocumentResponse
                {
                    DocumentId = documentId,
                    PresignedDownloadUrl = downloadUrl,
                    FileName = fileName,
                    MimeType = document.MimeType ?? "application/octet-stream",
                    FileSizeBytes = sizeBytes ?? 0,
                    ExpiresInMinutes = _apiOptions.PresignedUrlExpirationMinutes,
                    Status = document.ValidationStatus?.ToString() ?? "Available"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando URL de descarga para documento: {DocumentId}", documentId);
                throw;
            }
        }
    }
}
