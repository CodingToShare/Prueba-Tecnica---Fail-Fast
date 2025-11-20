using Erp.Documents.Infrastructure.Audit;
using Microsoft.Extensions.Logging;

namespace Erp.Documents.Infrastructure.Audit
{
    /// <summary>
    /// Implementación de IAuditService.
    /// Registra operaciones en memoria (para demostración).
    /// En producción, se debe persisitir en base de datos.
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly ILogger<AuditService> _logger;
        private static readonly List<AuditLog> AuditLogs = new();

        public AuditService(ILogger<AuditService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Registra una operación de auditoría.
        /// </summary>
        public async Task LogOperationAsync(
            Guid documentId,
            string operationType,
            string userId,
            string description,
            bool success,
            string? errorMessage = null)
        {
            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                DocumentId = documentId,
                OperationType = operationType,
                UserId = userId,
                Description = description,
                Success = success,
                ErrorMessage = errorMessage,
                CreatedAtUtc = DateTime.UtcNow,
                IpAddress = "127.0.0.1", // Se obtendría del contexto HTTP en un caso real
                UserAgent = "Application/1.0" // Se obtendría del header User-Agent
            };

            AuditLogs.Add(log);

            _logger.LogInformation(
                "Auditoría registrada - Operación: {OperationType}, DocumentId: {DocumentId}, UserId: {UserId}, Success: {Success}",
                operationType,
                documentId,
                userId,
                success);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Obtiene el historial de auditoría de un documento.
        /// </summary>
        public async Task<List<AuditLog>> GetDocumentAuditHistoryAsync(Guid documentId)
        {
            var history = AuditLogs
                .Where(log => log.DocumentId == documentId)
                .OrderByDescending(log => log.CreatedAtUtc)
                .ToList();

            _logger.LogInformation(
                "Historial de auditoría obtenido para DocumentId: {DocumentId}, Registros: {Count}",
                documentId,
                history.Count);

            return await Task.FromResult(history);
        }
    }
}
