using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipkart.Application.DTOs.Products;
using Shipkart.Domain.Enums;

namespace Shipkart.Application.Interfaces
{

    public interface IProductService
    {
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<IEnumerable<ProductDto>> GetFilteredProductsAsync(string? query, decimal? minPrice, decimal? maxPrice, bool? inStock, string? sku, TargetAudience? audience);
        Task<bool> UpdateProductAsync(Guid id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(Guid id);
    }
}
