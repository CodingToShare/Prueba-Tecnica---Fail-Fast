using Erp.Documents.Domain.Enums;

namespace Erp.Documents.Domain.Entities
{
    /// <summary>
    /// Entidad DocumentValidationFlow: flujo jerárquico de validación de un documento.
    /// </summary>
    public class DocumentValidationFlow
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public ValidationStatus? Status { get; set; } // P, A, R o null si sin validación
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        // Relaciones
        public Document? Document { get; set; }
        public ICollection<ValidationStep> Steps { get; set; } = new List<ValidationStep>();
        public ICollection<ValidationAction> Actions { get; set; } = new List<ValidationAction>();
    }
}
