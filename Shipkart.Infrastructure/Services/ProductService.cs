using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipkart.Application.DTOs.Products;
using Shipkart.Application.Exceptions;
using Shipkart.Application.Interfaces;
using Shipkart.Domain.Entities;

namespace Shipkart.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            // Manual validation if needed
            if (dto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value);
                if (category == null)
                    throw new AppException("Invalid category ID.", 400);

            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                Sku = dto.Sku,
                CategoryId = dto.CategoryId,
                IsPublished = dto.IsPublished,
                Color = dto.Color,
                Manufacturer = dto.Manufacturer,
                DeliveryEstimate = dto.DeliveryEstimate,
            };

            await _productRepository.AddAsync(product);

            return MapToDto(product);
        }


        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product is null ? null : MapToDto(product);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);
        }
        public async Task<IEnumerable<ProductDto>> GetFilteredProductsAsync(string? query, decimal? minPrice, decimal? maxPrice, bool? inStock, string? sku)
        {
            var products = await _productRepository.GetFilteredAsync(query, minPrice, maxPrice, inStock, sku);
            return products.Select(MapToDto);
        }

        public async Task<bool> UpdateProductAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null) return false;

            if (dto.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.CategoryId.Value);
                if (category == null)
                    throw new AppException("Invalid category ID.", 400);
            }

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.Sku = dto.Sku;
            product.UpdatedAt = DateTime.UtcNow;
            product.CategoryId = dto.CategoryId;
            product.IsPublished = dto.IsPublished;
            product.Color = dto.Color;
            product.Manufacturer = dto.Manufacturer;
            product.DeliveryEstimate = dto.DeliveryEstimate;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null) return false;

            await _productRepository.DeleteAsync(product);
            return true;
        }

        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Sku = product.Sku,
                CreatedAt = product.CreatedAt,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                CategorySlug = product.Category?.Slug,
                IsPublished = product.IsPublished,
                Color = product.Color,
                Manufacturer = product.Manufacturer,
                DeliveryEstimate = product.DeliveryEstimate,
            };
        }
    }
}
