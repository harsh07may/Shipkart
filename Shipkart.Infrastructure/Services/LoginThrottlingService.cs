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

        /// <summary>
        /// Checks if a user's email is currently locked out due to too many failed login attempts.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>True if the email is locked out, otherwise false.</returns>
        public Task<bool> IsLockedOutAsync(string email)
        {
            return Task.FromResult(_cache.TryGetValue(("lockout", email), out _));
        }

        /// <summary>
        /// Registers a failed login attempt for a given email address.
        /// If the number of failed attempts exceeds the maximum, the email will be locked out.
        /// </summary>
        /// <param name="email">The email address for which to register the failed attempt.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
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
        /// <summary>
        /// Resets the failed login attempts and lockout status for a given email address.
        /// This should be called upon a successful login.
        /// </summary>
        /// <param name="email">The email address for which to reset attempts.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public Task ResetAttemptsAsync(string email)
        {
            _cache.Remove(("attempt", email));
            _cache.Remove(("lockout", email));
            return Task.CompletedTask;
        }

    }
}
