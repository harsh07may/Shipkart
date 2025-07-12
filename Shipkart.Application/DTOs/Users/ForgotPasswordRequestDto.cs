using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipkart.Application.DTOs.Users
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = default!;
    }
}
