using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Domain.Exceptions;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IApplicationDbContext _context;
        public ProductService(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                CreatedDate = DateTime.UtcNow,
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync(new CancellationToken());

            return new ProductResponseDto
            {
                Id = product.Id,
                Description = product.Description,
                Name = product.Name,
                Price = product.Price
            };
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p =>  p.Id == id);
            if (product == null)
            {
                throw new NotFoundException(id);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(new CancellationToken());
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            return await _context.Products.AsNoTracking().Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description
            }).ToListAsync();
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(Guid id)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new NotFoundException(id);
            }
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price
            };

        }

        public async Task UpdateProductAsync(Guid id, ProductUpdateDto productDto)
        {

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                throw new NotFoundException(id);
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;

            await _context.SaveChangesAsync(new CancellationToken());
        }
    }
}
