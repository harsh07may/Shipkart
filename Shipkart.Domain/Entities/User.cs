    using Shipkart.Domain.Common;
using Shipkart.Domain.Enums;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Shipkart.Domain.Entities
    {   
        public class User : BaseEntity
        {
            public string FirstName { get; set; } = default!;
            public string LastName { get; set; } = default!;
            public string Email { get; set; } = default!;
            public string PasswordHash { get; set; } = default!;
            public UserRole Role { get; set; } = UserRole.Customer;
        }
    }
