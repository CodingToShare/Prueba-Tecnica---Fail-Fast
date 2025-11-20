using Erp.Documents.Domain.Enums;

namespace Erp.Documents.Domain.Entities
{
    /// <summary>
    /// Entidad ValidationStep: paso individual en el flujo de aprobación jerárquico.
    /// Mayor Order = mayor jerarquía.
    /// </summary>
    public class ValidationStep
    {
        public Guid Id { get; set; }
        public Guid FlowId { get; set; }
        public int Order { get; set; }
        public string ApproverUserId { get; set; } = string.Empty;
        public StepApprovalStatus Status { get; set; } = StepApprovalStatus.Pending;
        public DateTime? CompletedAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        // Relaciones
        public DocumentValidationFlow? Flow { get; set; }
    }
}
