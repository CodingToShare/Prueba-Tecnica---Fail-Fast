using Erp.Documents.Application.DTOs;
using Erp.Documents.Application.Interfaces;
using Erp.Documents.Domain.Entities;
using Erp.Documents.Domain.Enums;
using Erp.Documents.Infrastructure.Configuration;
using Erp.Documents.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Erp.Documents.Infrastructure.Services
{
    /// <summary>
    /// Implementación de IApproveDocumentService.
    /// Ejecuta lógica de aprobación jerárquica multi-paso.
    /// </summary>
    public class ApproveDocumentService : IApproveDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IValidationFlowRepository _validationFlowRepository;
        private readonly ValidationOptions _validationOptions;
        private readonly ILogger<ApproveDocumentService> _logger;

        public ApproveDocumentService(
            IDocumentRepository documentRepository,
            IValidationFlowRepository validationFlowRepository,
            IOptions<ValidationOptions> validationOptions,
            ILogger<ApproveDocumentService> logger)
        {
            _documentRepository = documentRepository;
            _validationFlowRepository = validationFlowRepository;
            _validationOptions = validationOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Aprueba un documento o avanza al siguiente paso de validación.
        /// </summary>
        public async Task<DocumentOperationResponse> ApproveAsync(ApproveDocumentRequest request)
        {
            try
            {
                // Obtener documento
                var document = await _documentRepository.GetByIdAsync(request.DocumentId);
                if (document == null)
                    throw new FileNotFoundException($"Documento {request.DocumentId} no encontrado");

                // Verificar que está en estado de validación
                if (document.ValidationStatus != ValidationStatus.P)
                    throw new InvalidOperationException(
                        $"Documento no puede ser aprobado. Estado: {document.ValidationStatus}");

                // Obtener flujo de validación
                if (!document.ValidationFlowId.HasValue)
                    throw new InvalidOperationException("No hay flujo de validación asociado");

                var validationFlow = await _validationFlowRepository.GetByIdAsync(document.ValidationFlowId.Value);
                if (validationFlow == null)
                    throw new InvalidOperationException("Flujo de validación no encontrado");

                // Obtener paso actual (el que está pendiente)
                var currentStep = validationFlow.Steps
                    .OrderBy(s => s.Order)
                    .FirstOrDefault(s => s.Status == StepApprovalStatus.Pending);

                if (currentStep == null)
                    throw new InvalidOperationException("No hay pasos pendientes de aprobación");

                // Marcar paso actual como aprobado
                currentStep.Status = StepApprovalStatus.Approved;
                currentStep.ApproverUserId = request.ApproverUserId;
                currentStep.CompletedAtUtc = DateTime.UtcNow;
                currentStep.UpdatedAtUtc = DateTime.UtcNow;

                // Registrar acción de aprobación
                var action = new ValidationAction
                {
                    Id = Guid.NewGuid(),
                    FlowId = validationFlow.Id,
                    StepId = currentStep.Id,
                    ActionType = ValidationActionType.Approve,
                    ActorUserId = request.ApproverUserId,
                    Reason = request.Reason,
                    CreatedAtUtc = DateTime.UtcNow
                };
                validationFlow.Actions.Add(action);

                // Verificar si hay más pasos pendientes
                var nextPendingStep = validationFlow.Steps
                    .OrderBy(s => s.Order)
                    .FirstOrDefault(s => s.Status == StepApprovalStatus.Pending);

                if (nextPendingStep == null)
                {
                    // No hay más pasos, documento está completamente aprobado
                    document.ValidationStatus = ValidationStatus.A;
                    document.UpdatedAtUtc = DateTime.UtcNow;

                    _logger.LogInformation(
                        "Documento completamente aprobado. DocumentId: {DocumentId}, ApproverUserId: {ApproverUserId}",
                        request.DocumentId,
                        request.ApproverUserId);
                }
                else
                {
                    _logger.LogInformation(
                        "Paso de validación aprobado. DocumentId: {DocumentId}, Step: {Step}, ApproverUserId: {ApproverUserId}",
                        request.DocumentId,
                        currentStep.Order,
                        request.ApproverUserId);
                }

                // Guardar cambios
                validationFlow.UpdatedAtUtc = DateTime.UtcNow;
                await _validationFlowRepository.UpdateAsync(validationFlow);
                await _documentRepository.UpdateAsync(document);

                return new DocumentOperationResponse
                {
                    DocumentId = request.DocumentId,
                    Status = document.ValidationStatus?.ToString() ?? "Approved",
                    Message = nextPendingStep == null
                        ? "Documento completamente aprobado"
                        : $"Paso {currentStep.Order} aprobado, pendiente paso {nextPendingStep.Order}",
                    OperatedAtUtc = DateTime.UtcNow,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error aprobando documento: {DocumentId}", request.DocumentId);
                throw;
            }
        }

        /// <summary>
        /// Obtiene estado del flujo de validación.
        /// </summary>
        public async Task<ValidationFlowStatusDto> GetValidationStatusAsync(Guid documentId)
        {
            try
            {
                var document = await _documentRepository.GetByIdAsync(documentId);
                if (document == null)
                    throw new FileNotFoundException($"Documento {documentId} no encontrado");

                var response = new ValidationFlowStatusDto
                {
                    DocumentId = documentId,
                    CurrentStatus = document.ValidationStatus?.ToString() ?? "NoValidation",
                    TotalSteps = 0,
                    CompletedSteps = 0,
                    Steps = new()
                };

                // Si no tiene flujo de validación, retornar respuesta vacía
                if (!document.ValidationFlowId.HasValue)
                    return response;

                var validationFlow = await _validationFlowRepository.GetByIdAsync(document.ValidationFlowId.Value);
                if (validationFlow == null)
                    return response;

                response.TotalSteps = validationFlow.Steps.Count;
                response.CompletedSteps = validationFlow.Steps.Count(s => s.Status == StepApprovalStatus.Approved);

                foreach (var step in validationFlow.Steps.OrderBy(s => s.Order))
                {
                    response.Steps.Add(new ValidationStepStatusDto
                    {
                        Order = step.Order,
                        Status = step.Status.ToString(),
                        ApproverUserId = Guid.TryParse(step.ApproverUserId, out var approverId) ? approverId : null,
                        ApprovedAtUtc = step.CompletedAtUtc,
                        Reason = validationFlow.Actions
                            .FirstOrDefault(a => a.StepId == step.Id && a.ActionType == ValidationActionType.Approve)
                            ?.Reason
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo estado de validación para documento: {DocumentId}", documentId);
                throw;
            }
        }
    }
}
