using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipkart.Application.DTOs.Products
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Sku { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategorySlug { get; set; }

        public bool IsPublished { get; set; }
        public string? Color { get; set; }
        public string? Manufacturer { get; set; }
        public string? DeliveryEstimate { get; set; }
    }
}
