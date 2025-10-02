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
using AutoMapper;

namespace ProductAPI.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ProductService(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return _mapper.Map<ProductResponseDto>(product);

        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new NotFoundException(id);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProductResponseDto>> GetAllProductsAsync()
        {
            var products = await _context.Products.AsNoTracking().ToListAsync();
            return _mapper.Map<List<ProductResponseDto>>(products);
        }

        public async Task<ProductResponseDto> GetProductByIdAsync(Guid id)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new NotFoundException(id);
            }
            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task PatchProductAsync(Guid id, ProductPatchDto productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                throw new NotFoundException(id);
            }

            _mapper.Map(productDto, product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Guid id, ProductUpdateDto productDto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                throw new NotFoundException(id);
            }

            _mapper.Map(productDto, product);
            await _context.SaveChangesAsync();
        }
    }
}
