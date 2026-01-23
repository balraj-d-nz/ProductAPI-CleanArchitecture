using System.Collections;
using System.Net;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Application.Common.Errors;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;
using ProductAPI.Domain.Entities;
using ProductAPI.Infrastructure.Persistence;
using ProductAPI.Swagger.Examples;
using Swashbuckle.AspNetCore.Filters;

namespace ProductAPI.Controllers
{
    /// <summary>
    /// API endpoints for managing products.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        /// <summary>
        /// Initializes a new instance of the ProductController class.
        /// </summary>
        /// <param name="productService">The product service.</param>
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>A list of products if any exist.</returns>
        [HttpGet]
        [Authorize(Policy = "ReadProduct")]
        [ProducesResponseType(typeof(List<ProductResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProductResponseDtoExample))]
        public async Task<ActionResult<List<ProductResponseDto>>> Get()
        {
            return Ok(await _productService.GetAllProductsAsync());
        }

        /// <summary>
        /// Gets a product by Id.
        /// </summary>
        /// <param name="id">The product Id.</param>
        /// <returns>The requested product.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProductResponseDtoExample))]
        public async Task<ActionResult<ProductResponseDto?>> Get(Guid id)
        {
            return Ok(await _productService.GetProductByIdAsync(id));
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="productDto">The product to create.</param>
        /// <returns>The newly created product.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ProductResponseDtoExample))]
        [SwaggerRequestExample(typeof(ProductCreateDto), typeof(ProductCreateDtoExample))]
        public async Task<ActionResult> Post(ProductCreateDto productDto)
        {
            var newProduct = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
        }

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="id">The Id of the product to update.</param>
        /// <param name="productDto">The updated Product object.</param>
        /// <returns>No content if successful</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerRequestExample(typeof(ProductUpdateDto), typeof(ProductUpdateDtoExample))]
        public async Task<ActionResult> Put(Guid id, ProductUpdateDto productDto)
        {
            await _productService.UpdateProductAsync(id, productDto);
            return NoContent();
        }

        /// <summary>
        /// Partially updates a product.
        /// </summary>
        /// <param name="id">The Id of the product to update.</param>
        /// <param name="patchDto">The updated Product object.</param>
        /// <returns>No content if successful.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerRequestExample(typeof(ProductPatchDto), typeof(ProductPatchDtoExample))]
        public async Task<ActionResult> Patch(Guid id, [FromBody] ProductPatchDto patchDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            await _productService.PatchProductAsync(id, patchDto);
            return NoContent();
        }



        /// <summary>
        /// Deletes a product.
        /// </summary>
        /// <param name="id">The Id of the product to delete.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
    }
}
