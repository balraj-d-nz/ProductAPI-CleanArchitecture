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
        Task<List<ProductResponseDto>> GetAllProductsAsync();
        Task<ProductResponseDto> GetProductByIdAsync(Guid id);
        Task<ProductResponseDto> CreateProductAsync(ProductCreateDto productDto);
        Task<ProductUpdateDto> GetProductForUpdateAsync(Guid id);
        Task UpdateProductAsync(Guid id, ProductUpdateDto productDto);
        Task PatchProductAsync(Guid id, ProductPatchDto productDto);
        Task DeleteProductAsync(Guid id);

    }
}
