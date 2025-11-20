namespace Erp.Documents.Application.Interfaces
{
    /// <summary>
    /// Interfaz para servicios de almacenamiento multi-cloud (Azure Blob, S3, etc).
    /// </summary>
    public interface IObjectStorageService
    {
        /// <summary>
        /// Genera una URL prefirmada (SAS o pre-signed) para subir un archivo.
        /// </summary>
        Task<string> GeneratePresignedUploadUrlAsync(string bucketKey, string mimeType, long sizeBytes, int expiresInMinutes);

        /// <summary>
        /// Genera una URL prefirmada para descargar un archivo.
        /// </summary>
        Task<string> GeneratePresignedDownloadUrlAsync(string bucketKey, int expiresInMinutes);

        /// <summary>
        /// Verifica si un objeto existe en el bucket.
        /// </summary>
        Task<bool> ObjectExistsAsync(string bucketKey);

        /// <summary>
        /// Obtiene los metadatos de un objeto (si es disponible).
        /// </summary>
        Task<(bool Exists, long? SizeBytes)> GetObjectMetadataAsync(string bucketKey);

        /// <summary>
        /// Elimina un objeto del bucket.
        /// </summary>
        Task DeleteObjectAsync(string bucketKey);
    }
}
