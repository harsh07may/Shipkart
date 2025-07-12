using Shipkart.Domain.Entities;

namespace Shipkart.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
