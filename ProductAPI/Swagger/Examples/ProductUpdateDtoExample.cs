using ProductAPI.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace ProductAPI.Swagger.Examples
{
    /// <summary>
    /// Example of what is sent when updating a product.
    /// </summary>
    public class ProductUpdateDtoExample : IExamplesProvider<ProductUpdateDto>
    {
        /// <summary>
        /// Example definition of object sent for product update.
        /// </summary>
        public ProductUpdateDto GetExamples()
        {
            return new ProductUpdateDto
            {
                Name = "2026 Speakers",
                Description = "New 2026 Model of Sony Speakers with amazing connectivity options and sound.",
                Price = 399.99m
            };
        }
    }
}
