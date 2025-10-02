using System.Net;
using System.Net.Http.Json;
using Azure;
using Microsoft.Extensions.DependencyInjection;
using ProductAPI.Application.DTOs;
using ProductAPI.Domain.Entities;
using ProductAPI.Infrastructure.Persistence;
using ProductAPI.IntegrationTests.Factory;

namespace ProductAPI.IntegrationTests
{
    // This tells xUnit to use our custom factory to bootstrap the tests
    public class ProductControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ProductControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient(); // Creates an HttpClient that can talk to our test server
        }

        [Fact]
        public async Task GetProductById_WithExistingId_ReturnsOkAndProduct()
        {
            var productId = Guid.NewGuid();

            // We need to access the database directly to seed data
            // We create a scope to get the DbContext instance
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                // Add a product to the in-memory database
                await context.Products.AddAsync(new Product { Id = productId, Name = "Test Product", Description = "Test Description", Price = 100, CreatedDate = DateTime.UtcNow });
                await context.SaveChangesAsync();
            }

            // ACT
            var resposne = await _client.GetAsync($"api/Product/{productId}");

            // ASSERT
            // 1. Check the HTTP response
            resposne.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, resposne.StatusCode);

            // 2. Deserialize the JSON response and check the data
            var productResponseDto = await resposne.Content.ReadFromJsonAsync<ProductResponseDto>();
            Assert.NotNull(productResponseDto);
            Assert.Equal(productId, productResponseDto.Id);
            Assert.Equal("Test Product", productResponseDto.Name);
        }
    }
}