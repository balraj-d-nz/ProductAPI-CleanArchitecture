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
            //Defines the Primary Key
            builder.HasKey(p => p.Id);

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

        }
    }
}
