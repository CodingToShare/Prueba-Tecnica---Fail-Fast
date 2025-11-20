namespace Erp.Documents.Application.DTOs
{
    /// <summary>
    /// Respuesta genérica de operación (Approve/Reject).
    /// </summary>
    public class DocumentOperationResponse
    {
        /// <summary>
        /// ID del documento.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Estado actual del documento.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Mensaje descriptivo.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp de la operación.
        /// </summary>
        public DateTime OperatedAtUtc { get; set; }

        /// <summary>
        /// Verdadero si la operación fue exitosa.
        /// </summary>
        public bool Success { get; set; }
    }
}
