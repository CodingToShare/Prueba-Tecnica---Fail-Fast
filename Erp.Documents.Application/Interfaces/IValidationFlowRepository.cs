using Erp.Documents.Domain.Entities;

namespace Erp.Documents.Application.Interfaces
{
    /// <summary>
    /// Interfaz para acceso a datos de flujos de validación.
    /// </summary>
    public interface IValidationFlowRepository
    {
        Task<DocumentValidationFlow?> GetByIdAsync(Guid id);
        Task<DocumentValidationFlow?> GetByDocumentIdAsync(Guid documentId);
        Task<DocumentValidationFlow> CreateAsync(DocumentValidationFlow flow);
        Task<DocumentValidationFlow> UpdateAsync(DocumentValidationFlow flow);
    }
}
