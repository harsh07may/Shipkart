using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipkart.Domain.Entities;
using Shipkart.Domain.Enums;

namespace Shipkart.Application.Interfaces
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetFilteredAsync(string? query, decimal? minPrice, decimal? maxPrice, bool? inStock, string? sku, TargetAudience? audience);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
