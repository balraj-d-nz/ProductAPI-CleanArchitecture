using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http.Json;
using Azure;
using Microsoft.EntityFrameworkCore;
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
        private readonly CustomWebApplicationFactory _factory;

        public ProductControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetProductByIdAsync_WithExistingId_ReturnsOkAndProduct()
        {
            var client = _factory.CreateClient(); // Creates an HttpClient that can talk to our test server
            var productId = Guid.NewGuid();

            // We need to access the database directly to seed data
            // We create a scope to get the DbContext instance
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                await context.Database.EnsureDeletedAsync(); // Ensure the DB is empty
                // Add a product to the in-memory database
                await context.Products.AddAsync(new Product { Id = productId, Name = "Test Product", Description = "Test Description", Price = 100, CreatedDate = DateTime.UtcNow });
                await context.SaveChangesAsync();
            }

            // ACT
            var response = await client.GetAsync($"api/Product/{productId}");

            // ASSERT
            // 1. Check the HTTP response
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // 2. Deserialize the JSON response and check the data
            var productResponseDto = await response.Content.ReadFromJsonAsync<ProductResponseDto>();
            Assert.NotNull(productResponseDto);
            Assert.Equal(productId, productResponseDto.Id);
            Assert.Equal("Test Product", productResponseDto.Name);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsOkAndProduct()
        {
            var client = _factory.CreateClient(); // Creates an HttpClient that can talk to our test server
            var createProductObjectAmount = 10; // How many products to create.
            // We need to access the database directly to seed data
            // We create a scope to get the DbContext instance
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                await context.Database.EnsureDeletedAsync(); // Ensure the DB is empty
                                                             // Add a products to the in-memory database

                for (int i = 1; i <= createProductObjectAmount; i++)
                {
                    await context.Products.AddAsync(new Product { Id = Guid.NewGuid(), Name = $"Test Product{i}", Description = $"Test Description{i}", Price = i * createProductObjectAmount, CreatedDate = DateTime.UtcNow });
                }
                await context.SaveChangesAsync();
            }

            // ACT
            var response = await client.GetAsync($"api/Product");

            // ASSERT
            // 1. Check the HTTP response
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // 2. Deserialize the JSON response and check the data
            var productListResponseDto = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>();
            Assert.NotNull(productListResponseDto);
            // Check that the API returned the number of products we created
            Assert.Equal(createProductObjectAmount, productListResponseDto.Count);
            Assert.Equal("Test Product1", productListResponseDto[0].Name);
        }

        [Fact]
        public async Task CreateNewProductAsync_ReturnsCreateAndProduct()
        {
            var productName = "Create Product 1";
            var productDescription = "Create Product 1";
            var productPrice = 99.99M;
            var client = _factory.CreateClient(); // Creates an HttpClient that can talk to our test server
            var createProductDto = new ProductCreateDto { Name = productName, Description = productDescription, Price = productPrice };

            // ACT
            var response = await client.PostAsJsonAsync($"api/Product", createProductDto);

            //Assert

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);

            //Check new product is in DB.

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var createdProduct = await context.Products.Where(p => p.Name.Equals(productName)).FirstOrDefaultAsync();
                Assert.NotNull(createdProduct);
                Assert.Equal(productPrice, createdProduct.Price);
                Assert.Equal(productDescription, createdProduct.Description);
            }
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsNoContent()
        {
            var productId = Guid.NewGuid();
            var productName = "Create Product 1";
            var productDescription = "Create Product 1";
            var productPrice = 99.99M;

            var client = _factory.CreateClient(); // Creates an HttpClient that can talk to our test server
            var productToPatch = new Product { Id = productId, Name = productName, Description = productDescription, Price = productPrice, CreatedDate = DateTime.UtcNow };

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                await context.Database.EnsureDeletedAsync(); // Ensure the DB is empty
                                                             // Add a products to the in-memory database

                await context.Products.AddAsync(productToPatch);
                await context.SaveChangesAsync();
            }

            var productUpdateDto = new ProductUpdateDto { Name = "Update Product 1", Description = productDescription, Price = productPrice };

            var response = await client.PutAsJsonAsync($"api/Product/{productId}", productUpdateDto);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var updatedProduct = await context.Products.FindAsync(productId);
                Assert.Equal("Update Product 1", updatedProduct?.Name);
                Assert.Equal(productDescription, updatedProduct?.Description);
                Assert.Equal(productPrice, updatedProduct?.Price);
            }
        }

        [Fact]
        public async Task PatchProductAsync_ReturnsNoContent()
        {
            var productId = Guid.NewGuid();
            var productName = "Create Product 1";
            var productDescription = "Create Product 1";
            var productPrice = 99.99M;

            var client = _factory.CreateClient(); // Creates an HttpClient that can talk to our test server
            var productToPatch = new Product { Id = productId, Name = productName, Description = productDescription, Price = productPrice, CreatedDate = DateTime.UtcNow };

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                await context.Database.EnsureDeletedAsync(); // Ensure the DB is empty
                                                             // Add a products to the in-memory database

                await context.Products.AddAsync(productToPatch);
                await context.SaveChangesAsync();
            }

            var productPatchDto = new ProductPatchDto { Name = "Patch Product 1" };

            var response = await client.PatchAsJsonAsync($"api/Product/{productId}", productPatchDto);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var patchedProduct = await context.Products.FindAsync(productId);
                Assert.Equal("Patch Product 1", patchedProduct?.Name);
                Assert.Equal(productDescription, patchedProduct?.Description);
                Assert.Equal(productPrice, patchedProduct?.Price);
            }
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnsNoContent()
        {
            var productId = Guid.NewGuid();
            var productName = "Create Product 1";
            var productDescription = "Create Product 1";
            var productPrice = 99.99M;

            var client = _factory.CreateClient(); // Creates an HttpClient that can talk to our test server
            var productToDelete = new Product { Id = productId, Name = productName, Description = productDescription, Price = productPrice, CreatedDate = DateTime.UtcNow };

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                await context.Database.EnsureDeletedAsync(); // Ensure the DB is empty
                                                             // Add a products to the in-memory database

                await context.Products.AddAsync(productToDelete);
                await context.SaveChangesAsync();
            }

            var response = await client.DeleteAsync($"api/Product/{productId}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var deletedProduct = await context.Products.FindAsync(productId);
                Assert.Null(deletedProduct);
            }
        }

        [Fact]
        public async Task GetProductByIdAsync_NotFoundException()
        {
            var productId = Guid.NewGuid();
            var client = _factory.CreateClient(); // Creates an HttpClient that can talk to our test server
            var response = await client.GetAsync($"api/Product/{productId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProductAsync_NotFoundException()
        {
            var productId = Guid.NewGuid();
            var productName = "Update Product 1";
            var productDescription = "Create Product 1";
            var productPrice = 99.99M;
            var client = _factory.CreateClient(); // Creates an HttpClient that can talk to our test server
            var productUpdateDto = new ProductUpdateDto { Name = productName, Description = productDescription, Price = productPrice };
            var response = await client.PutAsJsonAsync($"api/Product/{productId}", productUpdateDto);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PatchProductAsync_NotFoundException()
        {
            var productId = Guid.NewGuid();
            var client = _factory.CreateClient(); // Creates an HttpClient that can talk to our test server
            var productUpdateDto = new ProductPatchDto();
            var response = await client.PatchAsJsonAsync($"api/Product/{productId}", productUpdateDto);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProductAsync_NotFoundException()
        {
            var productId = Guid.NewGuid();
            var client = _factory.CreateClient(); // Creates an HttpClient that can talk to our test server
            var response = await client.DeleteAsync($"api/Product/{productId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}