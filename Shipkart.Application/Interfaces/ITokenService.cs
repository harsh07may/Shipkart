using Shipkart.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipkart.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
