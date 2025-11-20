using Erp.Documents.Domain.Enums;

namespace Erp.Documents.Domain.Entities
{
    /// <summary>
    /// Entidad ValidationAction: auditoría de acciones de aprobación/rechazo.
    /// </summary>
    public class ValidationAction
    {
        public Guid Id { get; set; }
        public Guid FlowId { get; set; }
        public Guid? StepId { get; set; } // Opcional: asociado a un paso específico
        public ValidationActionType ActionType { get; set; }
        public string ActorUserId { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        // Relaciones
        public DocumentValidationFlow? Flow { get; set; }
        public ValidationStep? Step { get; set; }
    }
}
