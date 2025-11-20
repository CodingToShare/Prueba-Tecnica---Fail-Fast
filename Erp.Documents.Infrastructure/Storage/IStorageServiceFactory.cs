using Erp.Documents.Application.Interfaces;
using Microsoft.Extensions.Options;
using Erp.Documents.Infrastructure.Configuration;

namespace Erp.Documents.Infrastructure.Storage
{
    /// <summary>
    /// Factory para crear instancias de IObjectStorageService basado en la configuración.
    /// Implementa el patrón Factory para soportar múltiples providers de almacenamiento.
    /// </summary>
    public interface IStorageServiceFactory
    {
        /// <summary>
        /// Crea una instancia de IObjectStorageService según el provider configurado.
        /// </summary>
        IObjectStorageService CreateStorageService();
    }

    public class StorageServiceFactory : IStorageServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly StorageOptions _storageOptions;

        public StorageServiceFactory(
            IServiceProvider serviceProvider,
            IOptions<StorageOptions> storageOptions)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _storageOptions = storageOptions.Value ?? throw new ArgumentNullException(nameof(storageOptions));
        }

        /// <summary>
        /// Crea una instancia del servicio de almacenamiento según Storage:Provider en appsettings.
        /// </summary>
        /// <returns>Instancia de IObjectStorageService (AzureBlobStorageService o S3StorageService)</returns>
        /// <exception cref="NotSupportedException">Si el provider no es soportado.</exception>
        public IObjectStorageService CreateStorageService()
        {
            var service = _storageOptions.Provider?.ToLowerInvariant() switch
            {
                "azureblob" => 
                    _serviceProvider.GetService(typeof(AzureBlobStorageService)) as IObjectStorageService,

                "awss3" =>
                    _serviceProvider.GetService(typeof(S3StorageService)) as IObjectStorageService,

                _ => null
            };

            if (service != null)
                return service;

            return _storageOptions.Provider?.ToLowerInvariant() switch
            {
                "azureblob" => throw new InvalidOperationException(
                    "AzureBlobStorageService no está registrado en DI."),

                "awss3" => throw new InvalidOperationException(
                    "S3StorageService no está registrado en DI."),

                _ => throw new NotSupportedException(
                    $"Storage provider '{_storageOptions.Provider}' no es soportado. " +
                    $"Use 'AzureBlob' o 'AwsS3'.")
            };
        }
    }
}
