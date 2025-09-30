using ProductAPI.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace ProductAPI.Swagger.Examples
{
    /// <summary>
    /// Example of what is sent when creating a new product.
    /// </summary>
    public class ProductCreateDtoExample : IExamplesProvider<ProductCreateDto>
    {
        /// <summary>
        /// Example definition of object sent for product creation.
        /// </summary>
        public ProductCreateDto GetExamples()
        {
            return new ProductCreateDto
            {
                Name = "High-Performance Speakers",
                Description = "The latest model with fantastic sound and bluetooth connectivity.",
                Price = 299.99m
            };
        }
    }
}
