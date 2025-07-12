namespace Shipkart.Domain.Common
{
    /// <summary>
    /// Represents the base entity class that provides common properties for all entities.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Auto-generated unique identifier.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Creation timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional update timestamp.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
