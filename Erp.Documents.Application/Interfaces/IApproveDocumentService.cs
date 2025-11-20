using Erp.Documents.Application.DTOs;

namespace Erp.Documents.Application.Interfaces
{
    /// <summary>
    /// Interfaz para servicio de validación/aprobación de documentos.
    /// Implementa lógica jerárquica de pasos de aprobación.
    /// </summary>
    public interface IApproveDocumentService
    {
        /// <summary>
        /// Aprueba un documento o avanza al siguiente paso de validación.
        /// Si es el último paso, marca como completamente aprobado.
        /// </summary>
        Task<DocumentOperationResponse> ApproveAsync(ApproveDocumentRequest request);

        /// <summary>
        /// Obtiene el estado actual del flujo de validación.
        /// </summary>
        Task<ValidationFlowStatusDto> GetValidationStatusAsync(Guid documentId);
    }
}
