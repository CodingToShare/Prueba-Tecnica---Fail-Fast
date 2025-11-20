using Erp.Documents.Application.DTOs;
using Erp.Documents.Application.Interfaces;
using Erp.Documents.Domain.Entities;
using Erp.Documents.Domain.Enums;
using Erp.Documents.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Erp.Documents.Infrastructure.Services
{
    /// <summary>
    /// Implementación de IRejectDocumentService.
    /// Rechaza documentos, marcándolos en estado terminal (Rejected).
    /// </summary>
    public class RejectDocumentService : IRejectDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IValidationFlowRepository _validationFlowRepository;
        private readonly ILogger<RejectDocumentService> _logger;

        public RejectDocumentService(
            IDocumentRepository documentRepository,
            IValidationFlowRepository validationFlowRepository,
            ILogger<RejectDocumentService> logger)
        {
            _documentRepository = documentRepository;
            _validationFlowRepository = validationFlowRepository;
            _logger = logger;
        }

        /// <summary>
        /// Rechaza un documento, registrando motivo y usuario que rechaza.
        /// </summary>
        public async Task<DocumentOperationResponse> RejectAsync(RejectDocumentRequest request)
        {
            try
            {
                // Obtener documento
                var document = await _documentRepository.GetByIdAsync(request.DocumentId);
                if (document == null)
                    throw new FileNotFoundException($"Documento {request.DocumentId} no encontrado");

                // Verificar que está en estado de validación (Pending)
                if (document.ValidationStatus != ValidationStatus.P)
                    throw new InvalidOperationException(
                        $"Documento no puede ser rechazado en estado: {document.ValidationStatus}. " +
                        $"Solo documentos en estado 'P (Pending)' pueden ser rechazados.");

                // Marcar como rechazado
                document.ValidationStatus = ValidationStatus.R;
                document.UpdatedAtUtc = DateTime.UtcNow;

                // Si tiene flujo de validación, registrar acción de rechazo
                if (document.ValidationFlowId.HasValue)
                {
                    var validationFlow = await _validationFlowRepository.GetByIdAsync(
                        document.ValidationFlowId.Value);

                    if (validationFlow != null)
                    {
                        // Marcar todos los pasos como rechazados
                        foreach (var step in validationFlow.Steps.Where(s => s.Status == StepApprovalStatus.Pending))
                        {
                            step.Status = StepApprovalStatus.Rejected;
                            step.UpdatedAtUtc = DateTime.UtcNow;
                        }

                        // Registrar acción de rechazo
                        var action = new ValidationAction
                        {
                            Id = Guid.NewGuid(),
                            FlowId = validationFlow.Id,
                            ActionType = ValidationActionType.Reject,
                            ActorUserId = request.RejecterUserId,
                            Reason = request.Reason,
                            CreatedAtUtc = DateTime.UtcNow
                        };
                        validationFlow.Actions.Add(action);
                        validationFlow.UpdatedAtUtc = DateTime.UtcNow;

                        await _validationFlowRepository.UpdateAsync(validationFlow);
                    }
                }

                // Guardar cambios
                await _documentRepository.UpdateAsync(document);

                _logger.LogInformation(
                    "Documento rechazado. DocumentId: {DocumentId}, RejecterUserId: {RejecterUserId}, Reason: {Reason}",
                    request.DocumentId,
                    request.RejecterUserId,
                    request.Reason);

                return new DocumentOperationResponse
                {
                    DocumentId = request.DocumentId,
                    Status = document.ValidationStatus?.ToString() ?? "Rejected",
                    Message = $"Documento rechazado. Motivo: {request.Reason}",
                    OperatedAtUtc = DateTime.UtcNow,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rechazando documento: {DocumentId}", request.DocumentId);
                throw;
            }
        }
    }
}
