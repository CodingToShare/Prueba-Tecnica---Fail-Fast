using Erp.Documents.Domain.Entities;

namespace Erp.Documents.Application.Interfaces
{
    /// <summary>
    /// Interfaz para acceso a datos de documentos.
    /// </summary>
    public interface IDocumentRepository
    {
        Task<Document?> GetByIdAsync(Guid id);
        Task<IEnumerable<Document>> GetByCompanyAndEntityAsync(Guid companyId, string entityType, string entityId);
        Task<Document> CreateAsync(Document document);
        Task<Document> UpdateAsync(Document document);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsByBucketKeyAsync(string bucketKey);
    }
}
