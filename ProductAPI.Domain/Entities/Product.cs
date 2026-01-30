namespace ProductAPI.Domain.Entities
{
    public class Product : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        
        // Navigation properties
        public User CreatedBy { get; set; } = null!;
        public User? UpdatedBy { get; set; }
        
    }
}
