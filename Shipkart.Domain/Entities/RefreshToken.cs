using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipkart.Domain.Common;

namespace Shipkart.Domain.Entities
{
    /// <summary>
    /// Represents a refresh token entity used for authentication and session management.
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user associated with this refresh token.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the refresh token string value.
        /// </summary>
        public string Token { get; set; } = default!;

        /// <summary>
        /// Gets or sets the expiration date and time of the refresh token.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the refresh token has been revoked.
        /// </summary>
        public bool IsRevoked { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the refresh token has been used.
        /// </summary>
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// Gets or sets the user associated with this refresh token.
        /// </summary>
        public User? User { get; set; }
    }
}

