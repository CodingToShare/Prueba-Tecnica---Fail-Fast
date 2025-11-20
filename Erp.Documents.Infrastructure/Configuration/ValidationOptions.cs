namespace Erp.Documents.Infrastructure.Configuration
{
    /// <summary>
    /// Opciones de validación de documentos.
    /// </summary>
    public class ValidationOptions
    {
        public const string SectionName = "Validation";
        
        public long MaxFileSizeBytes { get; set; } = 104857600; // 100 MB por defecto
        public List<string> AllowedMimeTypes { get; set; } = new();
    }
}
