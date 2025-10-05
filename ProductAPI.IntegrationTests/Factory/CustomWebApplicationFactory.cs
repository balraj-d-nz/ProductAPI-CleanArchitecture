using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductAPI.Application.Interfaces;
using ProductAPI.Infrastructure.Persistence;

namespace ProductAPI.IntegrationTests.Factory
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program> // It inherits from WebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Set the environment to "Testing"
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Find the DbContext registration
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

                // If found, remove it
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                // Also find the IApplicationDbContext registration if you have one
                var dbContextInterfaceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IApplicationDbContext));

                if (dbContextInterfaceDescriptor != null)
                {
                    services.Remove(dbContextInterfaceDescriptor);
                }

                // Add a new DbContext registration using an in-memory database
                services.AddDbContext<IApplicationDbContext, DatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForIntegrationTesting");
                });
            });
        }
    }
}
