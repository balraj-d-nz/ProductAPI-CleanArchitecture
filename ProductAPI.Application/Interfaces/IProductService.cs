using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductAPI.Application.DTOs;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto> GetProductByIdAsync();
        Task<int> CreateProductAsync(ProductCreateDto productDto);
        Task UpdateProductAsync(int id, ProductUpdateDto productDto);
        Task DeleteProductAsync(int id);

    }
}
