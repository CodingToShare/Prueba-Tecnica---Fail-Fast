using Erp.Documents.Application.DTOs;

namespace Erp.Documents.Application.Interfaces
{
    /// <summary>
    /// Interfaz para servicio de carga de documentos.
    /// Orquesta la creación de documento, generación de URL presignada y almacenamiento.
    /// </summary>
    public interface IUploadDocumentService
    {
        /// <summary>
        /// Inicia el proceso de carga de un documento.
        /// Crea el registro en BD y genera URL presignada para el cliente.
        /// </summary>
        Task<UploadDocumentResponse> InitiateUploadAsync(UploadDocumentRequest request);

        /// <summary>
        /// Completa/finaliza la carga después de que el cliente subió el archivo.
        /// Valida que el archivo existe en storage y actualiza estado del documento.
        /// </summary>
        Task<UploadDocumentResponse> CompleteUploadAsync(Guid documentId);
    }
}
