namespace ProductAPI.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CreatedById { get; set; } = string.Empty;
        public string? ModifiedById { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAtUtc { get; set; }
        
        // Navigation properties
        public User CreatedBy { get; set; } = null!;
        public User? UpdatedBy { get; set; }
        
    }
}
