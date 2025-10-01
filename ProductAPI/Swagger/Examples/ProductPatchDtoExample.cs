using Microsoft.AspNetCore.JsonPatch;
using ProductAPI.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace ProductAPI.Swagger.Examples
{
    /// <summary>
    /// Example of what is sent when partially updating a product.
    /// </summary>
    public class ProductPatchDtoExample : IExamplesProvider<ProductPatchDto>
    {
        /// <summary>
        /// Example definition of object sent for partial update.
        /// </summary>
        public ProductPatchDto GetExamples()
        {
            return new ProductPatchDto
            {
                Name = "New Desktop PC 2026",
                Price = 5999.99m
            };
        }
    }
}
