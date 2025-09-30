using ProductAPI.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace ProductAPI.Swagger.Examples
{
    /// <summary>
    /// Example response when fetching product data.
    /// </summary>
    public class ProductResponseDtoExample : IExamplesProvider<ProductResponseDto>
    {
        /// <summary>
        /// Example definition of response.
        /// </summary>
        public ProductResponseDto GetExamples()
        {
            return new ProductResponseDto
            {
                Id = new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"),
                Name = "High-Performance Laptop",
                Description = "The latest model with all the bells and whistles.",
                Price = 2499.99m
            };
        }
    }
}
