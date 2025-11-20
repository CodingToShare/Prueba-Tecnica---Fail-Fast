using Erp.Documents.Application.Interfaces;
using Erp.Documents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Erp.Documents.Infrastructure.Data
{
    /// <summary>
    /// Repositorio para la entidad DocumentValidationFlow.
    /// </summary>
    public class ValidationFlowRepository : IValidationFlowRepository
    {
        private readonly ErpDocumentsDbContext _context;

        public ValidationFlowRepository(ErpDocumentsDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentValidationFlow?> GetByIdAsync(Guid id)
        {
            return await _context.DocumentValidationFlows
                .Include(f => f.Document)
                .Include(f => f.Steps)
                .Include(f => f.Actions)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<DocumentValidationFlow?> GetByDocumentIdAsync(Guid documentId)
        {
            return await _context.DocumentValidationFlows
                .Include(f => f.Document)
                .Include(f => f.Steps)
                .Include(f => f.Actions)
                .FirstOrDefaultAsync(f => f.DocumentId == documentId);
        }

        public async Task<DocumentValidationFlow> CreateAsync(DocumentValidationFlow flow)
        {
            _context.DocumentValidationFlows.Add(flow);
            await _context.SaveChangesAsync();
            return flow;
        }

        public async Task<DocumentValidationFlow> UpdateAsync(DocumentValidationFlow flow)
        {
            _context.DocumentValidationFlows.Update(flow);
            await _context.SaveChangesAsync();
            return flow;
        }
    }
}
