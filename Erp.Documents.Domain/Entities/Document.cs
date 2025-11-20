using Erp.Documents.Domain.Enums;

namespace Erp.Documents.Domain.Entities
{
    /// <summary>
    /// Entidad Document: documento con metadatos en BD y archivo en cloud storage.
    /// </summary>
    public class Document
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string EntityType { get; set; } = string.Empty; // ej: "vehicle", "employee"
        public string EntityId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string BucketKey { get; set; } = string.Empty; // Ruta en el bucket (Azure/S3)
        public string? Hash { get; set; } // Opcional: SHA256, etc.
        public string CreatedByUserId { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        // Validación
        public ValidationStatus? ValidationStatus { get; set; }
        public Guid? ValidationFlowId { get; set; }

        // Relaciones
        public Company? Company { get; set; }
        public DocumentValidationFlow? ValidationFlow { get; set; }
    }
}
