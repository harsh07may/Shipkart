using Shipkart.Domain.Common;
using Shipkart.Domain.Enums;

namespace Shipkart.Domain.Entities
{
    /// <summary>
    /// Represents a user entity in the Shipkart system.
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; } = default!;

        /// <summary>
        /// Last name of the user.
        /// </summary>
        public string LastName { get; set; } = default!;

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// Hashed password of the user.
        /// </summary>
        public string PasswordHash { get; set; } = default!;

        /// <summary>
        /// Role of the user in the system.
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Customer;
    }
}
