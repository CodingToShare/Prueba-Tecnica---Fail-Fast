using Erp.Documents.Application.Interfaces;
using Erp.Documents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Erp.Documents.Infrastructure.Data
{
    /// <summary>
    /// Repositorio para la entidad Document.
    /// </summary>
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ErpDocumentsDbContext _context;

        public DocumentRepository(ErpDocumentsDbContext context)
        {
            _context = context;
        }

        public async Task<Document?> GetByIdAsync(Guid id)
        {
            return await _context.Documents
                .Include(d => d.Company)
                .Include(d => d.ValidationFlow)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Document>> GetByCompanyAndEntityAsync(Guid companyId, string entityType, string entityId)
        {
            return await _context.Documents
                .Include(d => d.Company)
                .Include(d => d.ValidationFlow)
                .Where(d => d.CompanyId == companyId && d.EntityType == entityType && d.EntityId == entityId)
                .ToListAsync();
        }

        public async Task<Document> CreateAsync(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<Document> UpdateAsync(Document document)
        {
            _context.Documents.Update(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task DeleteAsync(Guid id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document != null)
            {
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByBucketKeyAsync(string bucketKey)
        {
            return await _context.Documents.AnyAsync(d => d.BucketKey == bucketKey);
        }
    }
}
