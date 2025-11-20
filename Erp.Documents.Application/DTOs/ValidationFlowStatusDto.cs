namespace Erp.Documents.Application.DTOs
{
    /// <summary>
    /// DTO que representa el estado del flujo de validación de un documento.
    /// </summary>
    public class ValidationFlowStatusDto
    {
        /// <summary>
        /// ID del documento.
        /// </summary>
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Estado actual del documento: P (Pending), A (Approved), R (Rejected), o NoValidation.
        /// </summary>
        public string CurrentStatus { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad total de pasos en el flujo.
        /// </summary>
        public int TotalSteps { get; set; }

        /// <summary>
        /// Cantidad de pasos completados (aprobados).
        /// </summary>
        public int CompletedSteps { get; set; }

        /// <summary>
        /// Lista de estados de cada paso.
        /// </summary>
        public List<ValidationStepStatusDto> Steps { get; set; } = new();
    }
}
