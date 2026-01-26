using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Id)
            .HasMaxLength(128)
            .IsRequired();
        
        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(u => u.Name)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.HasIndex(u => u.Email);
        
        // Navigation
        builder.HasMany(u => u.CreatedProducts)
            .WithOne(p => p.CreatedBy)
            .HasForeignKey(p => p.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}