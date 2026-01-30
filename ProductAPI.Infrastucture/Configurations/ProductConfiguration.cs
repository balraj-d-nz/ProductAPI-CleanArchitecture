using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Infrastructure.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            
            //Defines the Primary Key
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            //Configure Name Property Required and Max Length
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);


            //Configure Description Property Max Length
            builder.Property(p => p.Description)
                .HasMaxLength(150);

            //Configure Price Property Precision
            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(7, 2);
            
            // Relationships
            builder.HasOne(p => p.CreatedBy)
                .WithMany(u => u.CreatedProducts)
                .HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        
            builder.HasOne(p => p.UpdatedBy)
                .WithMany()
                .HasForeignKey(p => p.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);
        
            // Indexes
            builder.HasIndex(p => p.CreatedById);
            builder.HasIndex(p => p.CreatedAtUtc);

        }
    }
}
