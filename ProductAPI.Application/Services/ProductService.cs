using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;

namespace ProductAPI.Application.Services
{
    public class ProductService : IProductService
    {
        public Task<int> CreateProductAsync(ProductCreateDto productDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductResponseDto> GetProductByIdAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateProductAsync(int id, ProductUpdateDto productDto)
        {
            throw new NotImplementedException();
        }
    }
}
