using Erp.Documents.Domain.Entities;

namespace Erp.Documents.Infrastructure.Audit
{
    /// <summary>
    /// Interfaz para servicio de auditoría.
    /// Registra todas las operaciones realizadas sobre documentos.
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Registra una operación de auditoría.
        /// </summary>
        Task LogOperationAsync(
            Guid documentId,
            string operationType,
            string userId,
            string description,
            bool success,
            string? errorMessage = null);

        /// <summary>
        /// Obtiene el historial de auditoría de un documento.
        /// </summary>
        Task<List<AuditLog>> GetDocumentAuditHistoryAsync(Guid documentId);
    }

    /// <summary>
    /// Modelo de auditoría para almacenar en base de datos.
    /// </summary>
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string OperationType { get; set; } = string.Empty; // Upload, Approve, Reject, Download
        public string UserId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
    }
}
