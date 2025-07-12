using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipkart.Domain.Common;

namespace Shipkart.Domain.Entities
{
    /// <summary>
    /// Represents a password reset token entity used for user password recovery.
    /// </summary>
    public class PasswordResetToken : BaseEntity
    {
        /// <summary>
        /// Unique identifier of the user associated with this password reset token.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The password reset token string value.
        /// </summary>
        public string Token { get; set; } = default!;

        /// <summary>
        /// The expiration date and time of the password reset token.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Value indicating whether the password reset token has been used.
        /// </summary>
        public bool Used { get; set; } = false;

        /// <summary>
        /// User associated with this refresh token.
        /// </summary>
        public User? User { get; set; }
    }
}
