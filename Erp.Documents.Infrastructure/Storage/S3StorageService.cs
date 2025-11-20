using Amazon.S3;
using Amazon.S3.Model;
using Erp.Documents.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Erp.Documents.Infrastructure.Configuration;

namespace Erp.Documents.Infrastructure.Storage
{
    /// <summary>
    /// Implementación de IObjectStorageService para AWS S3.
    /// Proporciona generación de URLs presignadas y operaciones de objetos.
    /// </summary>
    public class S3StorageService : IObjectStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsS3StorageOptions _options;
        private readonly ILogger<S3StorageService> _logger;

        public S3StorageService(
            IAmazonS3 s3Client,
            IOptions<StorageOptions> storageOptions,
            ILogger<S3StorageService> logger)
        {
            _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
            _options = storageOptions.Value.AwsS3;
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_options.BucketName))
            {
                throw new InvalidOperationException(
                    "AWS S3 BucketName no configurado en appsettings.");
            }

            if (string.IsNullOrWhiteSpace(_options.Region))
            {
                throw new InvalidOperationException(
                    "AWS S3 Region no configurada en appsettings.");
            }

            _logger.LogInformation(
                "AWS S3 Storage inicializado. Bucket: {BucketName}, Region: {Region}",
                _options.BucketName,
                _options.Region);
        }

        /// <summary>
        /// Genera una URL presignada para subir un archivo (PUT).
        /// </summary>
        public async Task<string> GeneratePresignedUploadUrlAsync(
            string bucketKey,
            string mimeType,
            long sizeBytes,
            int expiresInMinutes)
        {
            try
            {
                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _options.BucketName,
                    Key = bucketKey,
                    Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                    Verb = HttpVerb.PUT,
                    ContentType = mimeType,
                };

                // Agregar condición de tamaño si es posible
                // Nota: S3 presigned URLs tienen limitaciones en las condiciones
                // Esto es más un ejemplo; algunos validaciones deben ocurrir server-side

                var presignedUrl = _s3Client.GetPreSignedURL(request);

                _logger.LogInformation(
                    "URL presignada de carga generada para S3 key: {Key}, expira en {ExpiresInMinutes} minutos",
                    bucketKey,
                    expiresInMinutes);

                return presignedUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generando URL presignada de carga para S3 key: {Key}",
                    bucketKey);
                throw;
            }
        }

        /// <summary>
        /// Genera una URL presignada para descargar un archivo (GET).
        /// </summary>
        public async Task<string> GeneratePresignedDownloadUrlAsync(
            string bucketKey,
            int expiresInMinutes)
        {
            try
            {
                // Verificar que el objeto existe
                var exists = await ObjectExistsAsync(bucketKey);
                if (!exists)
                {
                    throw new FileNotFoundException(
                        $"El objeto S3 '{bucketKey}' no existe en bucket '{_options.BucketName}'.");
                }

                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _options.BucketName,
                    Key = bucketKey,
                    Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                    Verb = HttpVerb.GET,
                };

                var presignedUrl = _s3Client.GetPreSignedURL(request);

                _logger.LogInformation(
                    "URL presignada de descarga generada para S3 key: {Key}, expira en {ExpiresInMinutes} minutos",
                    bucketKey,
                    expiresInMinutes);

                return presignedUrl;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generando URL presignada de descarga para S3 key: {Key}",
                    bucketKey);
                throw;
            }
        }

        /// <summary>
        /// Verifica si un objeto existe en el bucket S3.
        /// </summary>
        public async Task<bool> ObjectExistsAsync(string bucketKey)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _options.BucketName,
                    Key = bucketKey,
                };

                try
                {
                    await _s3Client.GetObjectMetadataAsync(request);
                    return true;
                }
                catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error verificando existencia del objeto S3: {Key}",
                    bucketKey);
                throw;
            }
        }

        /// <summary>
        /// Obtiene metadatos de un objeto S3 (existencia y tamaño).
        /// </summary>
        public async Task<(bool Exists, long? SizeBytes)> GetObjectMetadataAsync(string bucketKey)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _options.BucketName,
                    Key = bucketKey,
                };

                try
                {
                    var response = await _s3Client.GetObjectMetadataAsync(request);
                    return (true, response.ContentLength);
                }
                catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error obteniendo metadatos del objeto S3: {Key}",
                    bucketKey);
                throw;
            }
        }

        /// <summary>
        /// Elimina un objeto del bucket S3.
        /// </summary>
        public async Task DeleteObjectAsync(string bucketKey)
        {
            try
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = bucketKey,
                };

                await _s3Client.DeleteObjectAsync(request);

                _logger.LogInformation(
                    "Objeto S3 eliminado: {Key}",
                    bucketKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error eliminando objeto S3: {Key}",
                    bucketKey);
                throw;
            }
        }
    }
}
