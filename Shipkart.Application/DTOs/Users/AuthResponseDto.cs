using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipkart.Application.DTOs.Users
{
    public class AuthResponseDto
    {
        public string Name { get; set; } = default!;
        public string Token { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}
