using System.Net.Sockets;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;
using ProductAPI.Application.Services;
using ProductAPI.Domain.Entities;
using ProductAPI.Domain.Exceptions;

namespace ProductAPI.Application.UnitTests
{
    public class ProductServiceTests
    {
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockMapper = new Mock<IMapper>();
            _productService = new ProductService(_mockContext.Object, _mockMapper.Object);
        }
        //Follow Arrange, Act, and Assert pattern when creating tests

        [Fact]
        public async Task GetProductByIdAsync_WithExistingId_Success()
        {
            var mockProductId = Guid.CreateVersion7();
            //1. Create fake mock data
            var mockProduct = new Product
            {
                Id = mockProductId, Name = "Test Product", Description = "Test Description", Price = 9.99M,
                CreatedOnUtc = DateTime.UtcNow
            };
            var mockProductDto = new ProductResponseDto
                { Id = mockProductId, Name = "Test Product", Description = "Test Description", Price = 9.99m };

            //2. Generate Entities and then Configure the DbContext mock for DbSet
            // Create a list containing our fake entity
            List<Product> mockProductsList = new List<Product> { mockProduct };

            // Configure the DbContext mock using Moq.EntityFrameworkCore
            _mockContext.Setup(x => x.Products).ReturnsDbSet(mockProductsList);

            // 3. Configure the Mapper mock
            // Tell the mapper: "When you are asked to map this specific entity and then return this specfic DTO.

            _mockMapper.Setup(m => m.Map<ProductResponseDto>(mockProduct)).Returns(mockProductDto);

            //4. Call the method we want to test. In this case its GetProductByIdAsync from ProductService
            var result = await _productService.GetProductByIdAsync(mockProductId);

            //5. Check the results
            Assert.NotNull(result);
            Assert.IsType<ProductResponseDto>(result);
            Assert.Equal(mockProductId, result.Id);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithNonExistentId_ThrowsNotFoundException()
        {
            var mockProductId = Guid.CreateVersion7();

            //1. Generate Entities and then Configure the DbContext mock for DbSet
            // Create an empty list to simulate not products so when it searches for product with specific Guid it wont find it and return NotFound
            List<Product> mockProductsList = new List<Product>();

            // Configure the DbContext mock using Moq.EntityFrameworkCore
            _mockContext.Setup(x => x.Products).ReturnsDbSet(mockProductsList);

            //3. Call the method we want to test in Assert. In this case its GetProductByIdAsync from ProductService but should return Exception NotFound.
            await Assert.ThrowsAsync<NotFoundException>(() => _productService.GetProductByIdAsync(mockProductId));
        }


        [Fact]
        public async Task CreateProductAsync_Success()
        {
            // 1. Prepare all the fake data objects we'll need for this test.
            var mockProductId = Guid.CreateVersion7();
            var mockProductCreateDto = new ProductCreateDto
                { Name = "Create Test Product", Description = "Create Test Description", Price = 29.99M };
            var mockProduct = new Product
            {
                Id = mockProductId, Name = mockProductCreateDto.Name, Description = mockProductCreateDto.Description,
                Price = mockProductCreateDto.Price, CreatedOnUtc = DateTime.UtcNow
            };
            var mockProductResponseDto = new ProductResponseDto
            {
                Id = mockProduct.Id, Name = mockProduct.Name, Description = mockProduct.Description,
                Price = mockProduct.Price
            };

            // 2. Configure the first mapper call.
            // When the service tries to map the createDto to a Product entity.
            _mockMapper.Setup(m => m.Map<Product>(mockProductCreateDto))
                // return our fake productEntity.
                .Returns(mockProduct);

            // 3. Configure the second mapper call.
            // When the service tries to map the resulting productEntity to a responseDto...
            _mockMapper.Setup(m => m.Map<ProductResponseDto>(mockProduct))
                // return our fake responseDto.
                .Returns(mockProductResponseDto);

            // 4. Configure the DbContext mock.
            // We just need to ensure the Products DbSet exists.
            // The AddAsync method will be verified later, not configured here.
            _mockContext.Setup(c => c.Products).ReturnsDbSet(new List<Product>());

            // ACT
            var createResponse = await _productService.CreateProductAsync(mockProductCreateDto);

            // ASSERT
            // 5. Check the state of the returned object
            Assert.NotNull(createResponse);
            Assert.IsType<ProductResponseDto>(createResponse);
            Assert.Equal(mockProductId, createResponse.Id);
            Assert.Equal("Create Test Product", createResponse.Name);

            // 6. Verify the behavior of the dependencies
            _mockContext.Verify(v => v.Products.AddAsync(mockProduct, default), Times.Once);
            _mockContext.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_Success()
        {
            var mockProductId = Guid.CreateVersion7();
            var mockProductUpdateDto = new ProductUpdateDto
                { Name = "Update Test Product", Description = "Update Test Description", Price = 19.99M };
            var mockProduct = new Product
            {
                Id = mockProductId, Name = "Original Test Product", Description = "Original Test Description",
                Price = 29.99M
            };


            // 1. Create a mock of the DbSet. This will allow setup of FindAsync().
            var mockProductSet = new Mock<DbSet<Product>>();

            // 2. Set up FindAsync on the mock DbSet to return your fake product
            mockProductSet.Setup(m => m.FindAsync(mockProductId)).ReturnsAsync(mockProduct);

            // 3. Set up the DbContext's Products property to return your mocked DbSet
            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            await _productService.UpdateProductAsync(mockProductId, mockProductUpdateDto);

            _mockMapper.Verify(v => v.Map(mockProductUpdateDto, mockProduct), Times.Once);
            _mockContext.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_NotFoundException()
        {
            var mockProductId = Guid.CreateVersion7();
            var mockProductUpdateDto = new ProductUpdateDto
                { Name = "Update Test Product", Description = "Update Test Description", Price = 19.99M };

            // 1. Create a mock of the DbSet. This will allow setup of FindAsync().
            var mockProductSet = new Mock<DbSet<Product>>();

            // 2. Set up FindAsync on the mock DbSet to return your fake product
            mockProductSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((Product?)null);

            // 3. Set up the DbContext's Products property to return your mocked DbSet
            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _productService.UpdateProductAsync(mockProductId, mockProductUpdateDto));
        }

        [Fact]
        public async Task PatchProductAsync_NotFoundException()
        {
            var mockProductId = Guid.CreateVersion7();
            var mockProductPatchDto = new ProductPatchDto { Description = "Patch Test Description", Price = 19.99M };

            // 1. Create a mock of the DbSet. This will allow setup of FindAsync().
            var mockProductSet = new Mock<DbSet<Product>>();

            // 2. Set up FindAsync on the mock DbSet to return your fake product
            mockProductSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((Product?)null);

            // 3. Set up the DbContext's Products property to return your mocked DbSet
            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _productService.PatchProductAsync(mockProductId, mockProductPatchDto));
        }

        [Fact]
        public async Task PatchProductAsync_Success()
        {
            var mockProductId = Guid.CreateVersion7();
            var mockProductPatchDto = new ProductPatchDto { Description = "Patch Test Description" };
            var mockProduct = new Product
            {
                Id = mockProductId, Name = "Original Test Product", Description = "Original Test Description",
                Price = 29.99M
            };

            // 1. Create a mock of the DbSet. This will allow setup of FindAsync().
            var mockProductSet = new Mock<DbSet<Product>>();

            // 2. Set up FindAsync on the mock DbSet to return your fake product
            mockProductSet.Setup(m => m.FindAsync(mockProductId)).ReturnsAsync(mockProduct);

            // 3. Set up the DbContext's Products property to return your mocked DbSet
            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            await _productService.PatchProductAsync(mockProductId, mockProductPatchDto);

            _mockMapper.Verify(v => v.Map(mockProductPatchDto, mockProduct), Times.Once);
            _mockContext.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }
        
        [Fact]
        public async Task DeleteProductAsync_Success()
        {
            var mockProductId = Guid.CreateVersion7();
            var mockProduct = new Product
            {
                Id = mockProductId, Name = "Original Test Product", Description = "Original Test Description",
                Price = 29.99M
            };

            // 1. Create a mock of the DbSet. This will allow setup of FindAsync().
            var mockProductSet = new Mock<DbSet<Product>>();

            // 2. Set up FindAsync on the mock DbSet to return your fake product
            mockProductSet.Setup(m => m.FindAsync(mockProductId)).ReturnsAsync(mockProduct);

            // 3. Set up the DbContext's Products property to return your mocked DbSet
            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            await _productService.DeleteProductAsync(mockProductId);

            _mockContext.Verify(v => v.Products.Remove(mockProduct), Times.Once);
            _mockContext.Verify(v => v.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_NotFoundException()
        {
            var mockProductId = Guid.CreateVersion7();

            // 1. Create a mock of the DbSet. This will allow setup of FindAsync().
            var mockProductSet = new Mock<DbSet<Product>>();

            // 2. Set up FindAsync on the mock DbSet to return your fake product
            mockProductSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .ReturnsAsync((Product?)null);

            // 3. Set up the DbContext's Products property to return your mocked DbSet
            _mockContext.Setup(c => c.Products).Returns(mockProductSet.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => _productService.DeleteProductAsync(mockProductId));
        }
    }
}