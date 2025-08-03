using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipkart.Application.DTOs.Products;
using Shipkart.Application.Interfaces;
using Shipkart.Domain.Enums;

namespace Shipkart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpPost]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
        {
            var product = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductDto>> GetById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null) return NotFound();
            return Ok(product);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
            [FromQuery] string? q,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? inStock,
            [FromQuery] string? sku,
            [FromQuery] TargetAudience? audience
            )
        {
            var hasAnyFilter = !string.IsNullOrWhiteSpace(q) || minPrice.HasValue || maxPrice.HasValue || inStock.HasValue || !string.IsNullOrWhiteSpace(sku) || audience.HasValue;

            if (!hasAnyFilter)
                return Ok(await _productService.GetAllProductsAsync());

            var filtered = await _productService.GetFilteredProductsAsync(q, minPrice, maxPrice, inStock, sku, audience);
            return Ok(filtered);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            var updated = await _productService.UpdateProductAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
