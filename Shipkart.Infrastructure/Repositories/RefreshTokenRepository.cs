using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shipkart.Api;
using Shipkart.Application.Interfaces;
using Shipkart.Domain.Entities;

namespace Shipkart.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token && !r.IsUsed && !r.IsRevoked);
        }

        public async Task InvalidateAsync(RefreshToken token)
        {
            token.IsUsed = true;
            token.IsRevoked = true;
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllAsync(Guid userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(t => t.UserId == userId && t.IsRevoked == false && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
        }

    }
}
