namespace Erp.Documents.Application.DTOs
{
    /// <summary>
    /// Respuesta de carga de documento con URL presignada.
    /// </summary>
    public class UploadDocumentResponse
    {
        /// <summary>
        /// ID del documento creado.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// URL presignada para subir el archivo al storage.
        /// </summary>
        public string PresignedUploadUrl { get; set; } = string.Empty;

        /// <summary>
        /// Clave única del objeto en storage.
        /// </summary>
        public string BucketKey { get; set; } = string.Empty;

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
