using Shipkart.Domain.Common;
using Shipkart.Domain.Enums;

namespace Shipkart.Domain.Entities
{
    /// <summary>
    /// Represents a product entity in the Shipkart system.
    /// </summary>
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Sku { get; set; } = default!;
        public bool IsDeleted { get; set; } = false;

        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        // New Enriched Fields
        public string? Color { get; set; }
        public string? Manufacturer { get; set; }
        public string? DeliveryEstimate { get; set; }
        public bool IsPublished { get; set; } = false;

    }

}