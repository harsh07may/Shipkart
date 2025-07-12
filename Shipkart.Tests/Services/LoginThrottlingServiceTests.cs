using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Shipkart.Infrastructure.Services;

namespace Shipkart.Tests.Services
{
    public class LoginThrottlingServiceTests
    {
        private readonly IMemoryCache _cache;
        private readonly LoginThrottlingService _sut;

        public LoginThrottlingServiceTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _sut = new LoginThrottlingService(_cache);
        }

        [Fact]
        public async Task IsLockedOutAsync_ShouldReturnFalse_WhenNoLockoutExists()
        {
            var result = await _sut.IsLockedOutAsync("user@example.com");
            result.Should().BeFalse();
        }

        [Fact]
        public async Task RegisterFailedAttemptAsync_ShouldTriggerLockout_WhenThresholdExceeded()
        {
            var email = "test@locked.com";

            // Trigger 5 failed attempts (threshold)
            for (int i = 0; i < 5; i++)
            {
                await _sut.RegisterFailedAttemptAsync(email);
            }

            var locked = await _sut.IsLockedOutAsync(email);
            locked.Should().BeTrue();
        }

        [Fact]
        public async Task ResetAttemptsAsync_ShouldClearLockoutAndFailures()
        {
            var email = "reset@demo.com";

            for (int i = 0; i < 5; i++)
                await _sut.RegisterFailedAttemptAsync(email);

            var lockedBefore = await _sut.IsLockedOutAsync(email);
            lockedBefore.Should().BeTrue();

            await _sut.ResetAttemptsAsync(email);

            var lockedAfter = await _sut.IsLockedOutAsync(email);
            lockedAfter.Should().BeFalse();
        }

    }
}
