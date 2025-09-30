using Microsoft.AspNetCore.JsonPatch;
using ProductAPI.Application.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace ProductAPI.Swagger.Examples
{
    /// <summary>
    /// Example of what is sent when updating a product.
    /// </summary>
    public class ProductPatchDtoExample : IExamplesProvider<JsonPatchDocument<ProductUpdateDto>>
    {
        /// <summary>
        /// Example definition of object sent for product patch.
        /// </summary>
        public JsonPatchDocument<ProductUpdateDto> GetExamples()
        {
            var patchDoc = new JsonPatchDocument<ProductUpdateDto>();

            patchDoc.Replace(dto => dto.Name, "New Deskotp PC 2026");
            patchDoc.Replace(dto => dto.Price, 5999.99m);

            return patchDoc;
        }
    }
}
