using Erp.Documents.Application.DTOs;

namespace Erp.Documents.Application.Interfaces
{
    /// <summary>
    /// Interfaz para servicio de descarga de documentos.
    /// Genera URLs presignadas para acceso a archivos en storage.
    /// </summary>
    public interface IDownloadDocumentService
    {
        /// <summary>
        /// Genera URL presignada para descargar un documento.
        /// </summary>
        Task<DownloadDocumentResponse> GetDownloadUrlAsync(Guid documentId);
    }
}
