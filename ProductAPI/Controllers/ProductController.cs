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

namespace ProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly ILogger<ProductController> _logger;
        public ProductController(DatabaseContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>A list of products if any exist.</returns>
        /// <response code="200">Returns the list of products</response>
        /// <response code="404">If no products are found</response>
        /// <response code="400">If an unexpected error occurs</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> Get()
        {
            var products = await _context.Products.ToListAsync();
            if (products.Any())
            {
                var productResponseDto = products.Select(p => new ProductResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                });
                return Ok(productResponseDto);
            }
            return NotFound(new ApiError { Code = HttpStatusCode.NotFound.ToString(), Message = "Could not find any products" });
        }

        /// <summary>
        /// Gets a product by Id.
        /// </summary>
        /// <param name="id">The product Id.</param>
        /// <returns>The requested product.</returns>
        /// <response code="200">Returns a product</response>
        /// <response code="404">If no product is found</response>
        /// <response code="400">If an unexpected error occurs</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product?>> Get(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new ApiError { Code = HttpStatusCode.NotFound.ToString(), Message = $"Could not find product with Id {id}" });
            }
            return Ok(product);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">The product to create.</param>
        /// <returns>The newly created product.</returns>
        /// <response code="201">Successfully created a product</response>
        /// <response code="400">If the product is invalid or an error occurs</response>
        [HttpPost]
        public async Task<ActionResult> Post(Product product)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid product data: {ModelState}", ModelState);
                return BadRequest(new ApiError { Code = HttpStatusCode.BadRequest.ToString(), Message = "Invalid product data", Details = ModelState.ToString() });
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }

        /// <summary>
        /// Updates a product.
        /// </summary>
        /// <param name="id">The Id of the product to update.</param>
        /// <param name="product">The updated Product object.</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If product is successfully updated</response>
        /// <response code="400">If an unexpected error occurs or the Id of the product to update does not match the Id we are looking for</response>
        /// <response code="404">If product to update is not found</response>
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, Product product)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid product data: {ModelState}", ModelState);
                return BadRequest(new ApiError { Code = HttpStatusCode.BadRequest.ToString(), Message = "Invalid product data", Details = ModelState.ToString() });
            }

            if (id != product.Id)
            {
                return BadRequest(new ApiError { Code = HttpStatusCode.BadRequest.ToString(), Message = $"Id ({id}) does not match Product Object Id ({product.Id})" });
            }
            if (await _context.Products.FindAsync(id) == null)
            {
                return NotFound(new ApiError { Code = HttpStatusCode.NotFound.ToString(), Message = $"Could not find product: {id}" });
            }
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Partially updates a product.
        /// </summary>
        /// <param name="id">The Id of the product to update.</param>
        /// <param name="patchProduct">The updated Product object.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If product is successfully updated</response>
        /// <response code="400">If an unexpected error occurs or the modelstate is not valid</response>
        /// <response code="404">If product to update is not found</response>
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(Guid id, JsonPatchDocument<Product> patchProduct)
        {
            var findProduct = await _context.Products.FindAsync(id);
            if (findProduct == null)
            {
                return NotFound(new ApiError { Code = HttpStatusCode.NotFound.ToString(), Message = $"Could not find product: {id}" });
            }
            patchProduct.ApplyTo(findProduct);
            if (!TryValidateModel(findProduct))
            {
                return BadRequest(new ApiError { Code = HttpStatusCode.BadRequest.ToString(), Message = $"ModelState is not valid during patch action. ModelState: {ModelState.ToString()}" });

            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Deletes a product.
        /// </summary>
        /// <param name="id">The Id of the product to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If product is successfully deleted</response>
        /// <response code="400">If an unexpected error occurs</response>
        /// <response code="404">If product to delete is not found</response>
        [HttpDelete]
        public async Task<ActionResult> Delete(Guid id)
        {
            var findProduct = await _context.Products.FindAsync(id);
            if (findProduct != null)
            {
                _context.Remove(findProduct);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound(new ApiError { Code = HttpStatusCode.NotFound.ToString(), Message = $"Could not find product to delete: {id}" });
        }
    }
}
