using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Erp.Documents.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Erp.Documents.Infrastructure.Configuration;

namespace Erp.Documents.Infrastructure.Storage
{
    /// <summary>
    /// Implementación de IObjectStorageService para Azure Blob Storage.
    /// Proporciona generación de URLs presignadas (SAS) y operaciones de objetos.
    /// </summary>
    public class AzureBlobStorageService : IObjectStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly AzureStorageOptions _options;
        private readonly Azure.Storage.StorageSharedKeyCredential? _sharedKeyCredential;
        private readonly ILogger<AzureBlobStorageService> _logger;

        public AzureBlobStorageService(
            IOptions<StorageOptions> storageOptions,
            ILogger<AzureBlobStorageService> logger)
        {
            _options = storageOptions.Value.Azure;
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
            {
                throw new InvalidOperationException(
                    "Azure Storage ConnectionString no configurada en appsettings.");
            }

            if (string.IsNullOrWhiteSpace(_options.ContainerName))
            {
                throw new InvalidOperationException(
                    "Azure Storage ContainerName no configurada en appsettings.");
            }

            var blobServiceClient = new BlobServiceClient(_options.ConnectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(_options.ContainerName);

            // Extraer credenciales para generar SAS tokens
            _sharedKeyCredential = ExtractCredentialsFromConnectionString(_options.ConnectionString);

            _logger.LogInformation(
                "Azure Blob Storage inicializado. Container: {ContainerName}",
                _options.ContainerName);
        }

        /// <summary>
        /// Genera una URL SAS presignada para subir un archivo (PUT).
        /// </summary>
        public async Task<string> GeneratePresignedUploadUrlAsync(
            string bucketKey,
            string mimeType,
            long sizeBytes,
            int expiresInMinutes)
        {
            try
            {
                if (_sharedKeyCredential == null)
                    throw new InvalidOperationException("No se pudieron extraer credenciales de la connection string");

                var blobClient = _containerClient.GetBlobClient(bucketKey);

                var expiryTime = DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes);
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _options.ContainerName,
                    BlobName = bucketKey,
                    ExpiresOn = expiryTime,
                    Protocol = SasProtocol.Https
                };

                // Permisos: Create, Write, Delete (para carga)
                sasBuilder.SetPermissions("cwd");

                var sasToken = sasBuilder.ToSasQueryParameters(_sharedKeyCredential).ToString();
                var sasUrl = new UriBuilder(blobClient.Uri)
                {
                    Query = sasToken
                }.Uri.ToString();

                _logger.LogInformation(
                    "URL SAS de carga generada para blob: {BlobName}, expira en {ExpiresInMinutes} minutos",
                    bucketKey,
                    expiresInMinutes);

                return sasUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generando URL SAS de carga para blob: {BlobName}",
                    bucketKey);
                throw;
            }
        }

        /// <summary>
        /// Genera una URL SAS presignada para descargar un archivo (GET).
        /// </summary>
        public async Task<string> GeneratePresignedDownloadUrlAsync(
            string bucketKey,
            int expiresInMinutes)
        {
            try
            {
                if (_sharedKeyCredential == null)
                    throw new InvalidOperationException("No se pudieron extraer credenciales de la connection string");

                var blobClient = _containerClient.GetBlobClient(bucketKey);

                // Verificar que el blob existe
                var exists = await blobClient.ExistsAsync();
                if (!exists.Value)
                {
                    throw new FileNotFoundException($"El blob '{bucketKey}' no existe.");
                }

                var expiryTime = DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes);
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _options.ContainerName,
                    BlobName = bucketKey,
                    ExpiresOn = expiryTime,
                    Protocol = SasProtocol.Https
                };

                // Permisos: Read (para descarga)
                sasBuilder.SetPermissions("r");

                var sasToken = sasBuilder.ToSasQueryParameters(_sharedKeyCredential).ToString();
                var sasUrl = new UriBuilder(blobClient.Uri)
                {
                    Query = sasToken
                }.Uri.ToString();

                _logger.LogInformation(
                    "URL SAS de descarga generada para blob: {BlobName}, expira en {ExpiresInMinutes} minutos",
                    bucketKey,
                    expiresInMinutes);

                return sasUrl;
            }
            catch (FileNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error generando URL SAS de descarga para blob: {BlobName}",
                    bucketKey);
                throw;
            }
        }

        /// <summary>
        /// Verifica si un blob existe en el contenedor.
        /// </summary>
        public async Task<bool> ObjectExistsAsync(string bucketKey)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(bucketKey);
                var exists = await blobClient.ExistsAsync();
                return exists.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error verificando existencia del blob: {BlobName}",
                    bucketKey);
                throw;
            }
        }

        /// <summary>
        /// Obtiene metadatos de un blob (existencia y tamaño).
        /// </summary>
        public async Task<(bool Exists, long? SizeBytes)> GetObjectMetadataAsync(string bucketKey)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(bucketKey);
                var exists = await blobClient.ExistsAsync();

                if (!exists.Value)
                {
                    return (false, null);
                }

                var properties = await blobClient.GetPropertiesAsync();
                return (true, properties.Value.ContentLength);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error obteniendo metadatos del blob: {BlobName}",
                    bucketKey);
                throw;
            }
        }

        /// <summary>
        /// Elimina un blob del contenedor.
        /// </summary>
        public async Task DeleteObjectAsync(string bucketKey)
        {
            try
            {
                var blobClient = _containerClient.GetBlobClient(bucketKey);
                await blobClient.DeleteIfExistsAsync();

                _logger.LogInformation(
                    "Blob eliminado: {BlobName}",
                    bucketKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error eliminando blob: {BlobName}",
                    bucketKey);
                throw;
            }
        }

        /// <summary>
        /// Extrae credenciales (StorageSharedKeyCredential) de la connection string.
        /// Formato: "DefaultEndpointsProtocol=https;AccountName=XXX;AccountKey=YYY;..."
        /// </summary>
        private static Azure.Storage.StorageSharedKeyCredential? ExtractCredentialsFromConnectionString(
            string connectionString)
        {
            try
            {
                var parts = connectionString.Split(';');
                var accountNamePart = parts.FirstOrDefault(p => p.StartsWith("AccountName="));
                var accountKeyPart = parts.FirstOrDefault(p => p.StartsWith("AccountKey="));

                if (accountNamePart == null || accountKeyPart == null)
                    return null;

                var accountName = accountNamePart.Substring("AccountName=".Length);
                var accountKey = accountKeyPart.Substring("AccountKey=".Length);

                return new Azure.Storage.StorageSharedKeyCredential(accountName, accountKey);
            }
            catch
            {
                return null;
            }
        }
    }
}
