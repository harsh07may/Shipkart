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

    }

}