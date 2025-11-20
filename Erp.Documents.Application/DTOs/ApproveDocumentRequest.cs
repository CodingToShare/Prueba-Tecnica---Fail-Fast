namespace Erp.Documents.Application.DTOs
{
    /// <summary>
    /// Solicitud para aprobar un documento en la validación.
    /// </summary>
    public class ApproveDocumentRequest
    {
        /// <summary>
        /// ID del documento a aprobar.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Comentario o motivo de aprobación.
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// ID del usuario que aprueba.
        /// </summary>
        public string ApproverUserId { get; set; } = string.Empty;
    }
}
