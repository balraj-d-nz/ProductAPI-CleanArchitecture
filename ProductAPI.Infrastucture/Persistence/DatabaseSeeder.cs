using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProductAPI.Infrastructure.Persistence
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDatabaseAsync(DatabaseContext context)
        {
            // The migration will create the database, so we can just ensure it exists.
            await context.Database.MigrateAsync();

            // Check if there is already data
            if (await context.Products.AnyAsync())
            {
                return; // DB has been seeded
            }

            var products = new List<Product>
        {
            new Product { Name = "Laptop", Price = 1999.99M, Description = "High performance laptop" },
            new Product { Name = "Smartphone", Price = 899.50M, Description = "Latest model smartphone" },
            new Product { Name = "Headphones", Price = 149.95M, Description = "Noise-cancelling headphones" },
            new Product { Name = "Keyboard", Price = 79.99M, Description = "Mechanical keyboard"},
            new Product { Name = "Monitor", Price = 499.00M, Description = "27-inch 4K monitor" }
        };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
