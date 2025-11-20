using Erp.Documents.Application.DTOs;

namespace Erp.Documents.Application.Interfaces
{
    /// <summary>
    /// Interfaz para servicio de rechazo de documentos.
    /// Marca documentos como rechazados (estado terminal).
    /// </summary>
    public interface IRejectDocumentService
    {
        /// <summary>
        /// Rechaza un documento, registrando motivo y usuario que rechaza.
        /// El estado pasa a Rejected (terminal, no se puede cambiar).
        /// </summary>
        Task<DocumentOperationResponse> RejectAsync(RejectDocumentRequest request);
    }
}
