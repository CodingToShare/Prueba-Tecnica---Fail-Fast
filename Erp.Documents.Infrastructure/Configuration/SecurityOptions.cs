namespace Erp.Documents.Infrastructure.Configuration
{
    /// <summary>
    /// Opciones de seguridad.
    /// </summary>
    public class SecurityOptions
    {
        public const string SectionName = "Security";
        
        public bool EnableUserValidation { get; set; } = true;
        public string UserIdHeaderName { get; set; } = "X-User-Id";
        public string CompanyIdHeaderName { get; set; } = "X-Company-Id";
    }
}
