using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Shipkart.Application.Interfaces;

namespace Shipkart.Infrastructure.Services
{
    public class LoginThrottlingService :   ILoginThrottlingService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan lockoutDuration = TimeSpan.FromMinutes(5);
        private readonly int maxAttempts = 5;

        public LoginThrottlingService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<bool> IsLockedOutAsync(string email)
        {
            return Task.FromResult(_cache.TryGetValue(("lockout", email), out _));
        }


        public Task RegisterFailedAttemptAsync(string email)
        {
            var key = ("attempt", email);
            var count = _cache.GetOrCreate<int>(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = lockoutDuration;
                return 0;
            });

            count++;
            if (count >= maxAttempts)
            {
                _cache.Set(("lockout", email), true, lockoutDuration);
                _cache.Remove(key);
            }
            else
            {
                _cache.Set(key, count, lockoutDuration);
            }

            return Task.CompletedTask;
        }
        public Task ResetAttemptsAsync(string email)
        {
            _cache.Remove(("attempt", email));
            _cache.Remove(("lockout", email));
            return Task.CompletedTask;
        }

    }
}
