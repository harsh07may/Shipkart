using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipkart.Domain.Entities;

namespace Shipkart.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task InvalidateAsync(RefreshToken token);
        Task RevokeAllAsync(Guid userId);

    }
}
