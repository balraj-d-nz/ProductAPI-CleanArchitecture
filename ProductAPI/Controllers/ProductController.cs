using System.Collections;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using ProductAPI.Domain.Entities;
using ProductAPI.Infrastructure.Persistence;
using System.Net;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Common.Errors;
using ProductAPI.Application.Interfaces;

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
        [ProducesResponseType(typeof(List<ProductResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        public async Task<ActionResult> Put(Guid id, ProductUpdateDto productDto)
        {
            await _productService.UpdateProductAsync(id, productDto);
            return NoContent();
        }

        /// <summary>
        /// Partially updates a product.
        /// </summary>
        /// <param name="id">The Id of the product to update.</param>
        /// <param name="patchDoc">The updated Product object.</param>
        /// <returns>No content if successful.</returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Patch(Guid id, JsonPatchDocument<ProductUpdateDto> patchDoc)
        {
            // 1. Get the original data as a DTO
            var productToUpdate = await _productService.GetProductByIdAsync(id);

            var productDto = new ProductUpdateDto
            {
                Name = productToUpdate.Name,
                Description = productToUpdate.Description,
                Price = productToUpdate.Price
            };

            // 2. Apply the patch to the DTO (without ModelState)
            patchDoc.ApplyTo(productDto);

            // 3. Manually re-validate the DTO after the patch is applied
            if (!TryValidateModel(productDto))
            {
                return ValidationProblem(ModelState);
            }

            // 4. Send the fully updated DTO to the service to save
            await _productService.UpdateProductAsync(id, productDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes a product.
        /// </summary>
        /// <param name="id">The Id of the product to delete.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete]
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
