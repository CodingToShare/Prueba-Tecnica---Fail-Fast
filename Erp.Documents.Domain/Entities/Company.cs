namespace Erp.Documents.Domain.Entities
{
    /// <summary>
    /// Entidad Company: representa una empresa en el ERP.
    /// </summary>
    public class Company
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }

        // Relaciones
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
