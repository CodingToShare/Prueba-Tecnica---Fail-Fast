namespace Erp.Documents.Infrastructure.Configuration
{
    /// <summary>
    /// Opciones de API.
    /// </summary>
    public class ApiOptions
    {
        public const string SectionName = "Api";
        
        public int PresignedUrlExpirationMinutes { get; set; } = 30;
        public bool RequireAuthHeaders { get; set; } = true;
    }
}
