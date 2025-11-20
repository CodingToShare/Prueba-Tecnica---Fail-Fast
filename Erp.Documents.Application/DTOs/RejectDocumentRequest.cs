namespace Erp.Documents.Application.DTOs
{
    /// <summary>
    /// Solicitud para rechazar un documento.
    /// </summary>
    public class RejectDocumentRequest
    {
        /// <summary>
        /// ID del documento a rechazar.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Motivo del rechazo.
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// ID del usuario que rechaza.
        /// </summary>
        public string RejecterUserId { get; set; } = string.Empty;
    }
}
