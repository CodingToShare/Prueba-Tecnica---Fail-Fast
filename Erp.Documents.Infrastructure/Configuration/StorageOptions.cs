namespace Erp.Documents.Infrastructure.Configuration
{
    /// <summary>
    /// Opciones de almacenamiento genérico.
    /// </summary>
    public class StorageOptions
    {
        public const string SectionName = "Storage";
        
        public string Provider { get; set; } = "AzureBlob"; // "AzureBlob" o "AwsS3"
        public AzureStorageOptions Azure { get; set; } = new();
        public AwsS3StorageOptions AwsS3 { get; set; } = new();
    }

    public class AzureStorageOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
        public int SasUrlExpirationMinutes { get; set; } = 30;
    }

    public class AwsS3StorageOptions
    {
        public string BucketName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public int PreSignedUrlExpirationMinutes { get; set; } = 30;
    }
}
