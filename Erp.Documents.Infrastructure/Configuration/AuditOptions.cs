namespace Erp.Documents.Infrastructure.Configuration
{
    /// <summary>
    /// Opciones de auditoría.
    /// </summary>
    public class AuditOptions
    {
        public const string SectionName = "Audit";
        
        public bool EnableAuditLog { get; set; } = true;
        public int RetentionDays { get; set; } = 365;
    }
}
