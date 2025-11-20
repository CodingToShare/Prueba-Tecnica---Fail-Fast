namespace Erp.Documents.Application.DTOs
{
    /// <summary>
    /// Solicitud para iniciar carga de documento.
    /// </summary>
    public class UploadDocumentRequest
    {
        /// <summary>
        /// ID de la compañía propietaria del documento.
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Tipo de entidad asociada (ej: "Invoice", "PurchaseOrder", "Contract").
        /// </summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// ID de la entidad asociada (ej: ID del invoice).
        /// </summary>
        public string EntityId { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del archivo.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// MIME type del archivo (ej: "application/pdf", "image/png").
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// Tamaño del archivo en bytes.
        /// </summary>
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// Requiere validación/aprobación antes de finalizar.
        /// </summary>
        public bool RequiresValidation { get; set; }

        /// <summary>
        /// ID del usuario que está subiendo el documento.
        /// </summary>
        public string? UploadedByUserId { get; set; }
    }
}
