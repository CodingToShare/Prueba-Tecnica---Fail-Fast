namespace Erp.Documents.Application.DTOs
{
    /// <summary>
    /// Respuesta de descarga de documento.
    /// </summary>
    public class DownloadDocumentResponse
    {
        /// <summary>
        /// ID del documento.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// URL presignada para descargar el archivo.
        /// </summary>
        public string PresignedDownloadUrl { get; set; } = string.Empty;

        /// <summary>
        /// Nombre original del archivo.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// MIME type del archivo.
        /// </summary>
        public string MimeType { get; set; } = string.Empty;

        /// <summary>
        /// Tamaño del archivo en bytes.
        /// </summary>
        public long FileSizeBytes { get; set; }

        /// <summary>
        /// Minutos restantes antes de que expire la URL.
        /// </summary>
        public int ExpiresInMinutes { get; set; }

        /// <summary>
        /// Estado actual del documento.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
