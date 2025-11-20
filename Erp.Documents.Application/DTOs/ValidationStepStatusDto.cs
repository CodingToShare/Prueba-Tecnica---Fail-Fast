namespace Erp.Documents.Application.DTOs
{
    /// <summary>
    /// DTO que representa el estado de un paso de validación.
    /// </summary>
    public class ValidationStepStatusDto
    {
        /// <summary>
        /// Orden/posición del paso en el flujo.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Estado del paso: Pending, Approved, Rejected.
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// ID del usuario que aprobó/rechazó el paso (si aplica).
        /// </summary>
        public Guid? ApproverUserId { get; set; }

        /// <summary>
        /// Fecha/hora cuando se completó el paso.
        /// </summary>
        public DateTime? ApprovedAtUtc { get; set; }

        /// <summary>
        /// Motivo o comentario del aprobador.
        /// </summary>
        public string? Reason { get; set; }
    }
}
