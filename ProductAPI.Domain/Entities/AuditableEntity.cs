namespace ProductAPI.Domain.Entities;

public class AuditableEntity
{
    public string CreatedById { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public string? ModifiedById { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
}