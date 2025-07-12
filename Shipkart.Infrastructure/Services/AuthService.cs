using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shipkart.Application.DTOs.Users;
using Shipkart.Application.Exceptions;
using Shipkart.Application.Interfaces;
using Shipkart.Domain.Entities;

namespace Shipkart.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly ILoginThrottlingService _loginThrottlingService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepo,
            ILoginThrottlingService loginThrottlingService,
            ILogger<AuthService> logger
            )
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _refreshTokenRepo = refreshTokenRepo;
            _loginThrottlingService = loginThrottlingService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user based on their email and password.
        /// </summary>
        /// <param name="dto">The user login data transfer object.</param>
        /// <returns>An authentication response containing access and refresh tokens.</returns>
        public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
        {
            if (await _loginThrottlingService.IsLockedOutAsync(dto.Email))
                throw new AppException("Too many failed login attempts. Try again later.", 429);

            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                await _loginThrottlingService.RegisterFailedAttemptAsync(dto.Email);
                throw new AppException("Invalid credentials", 401);
            }

            var accessToken = _tokenService.GenerateToken(user);

            var refreshToken = GenerateRefreshToken(user.Id);

            await _refreshTokenRepo.AddAsync(refreshToken);
            await _loginThrottlingService.ResetAttemptsAsync(dto.Email);


            return new AuthResponseDto
            {
                Email = user.Email,
                Token = accessToken,
                RefreshToken = refreshToken.Token,
            };
        }

        /// <summary>
        /// Refreshes an access token using a valid refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token string.</param>
        /// <returns>A new access token and refresh token.</returns>
        public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var oldToken = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
            if (oldToken == null || oldToken.ExpiresAt < DateTime.UtcNow)
                throw new AppException("Invalid or expired refresh token", 401);

            await _refreshTokenRepo.InvalidateAsync(oldToken);

            var user = oldToken.User!;
            var accessToken = _tokenService.GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken(user.Id);

            await _refreshTokenRepo.AddAsync(newRefreshToken);

            return new RefreshTokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token
            };
        }
        /// <summary>
        /// Logs out a user by invalidating a specific refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token to invalidate.</param>
        /// <returns>True if the logout was successful.</returns>
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var token = await _refreshTokenRepo.GetByTokenAsync(refreshToken);
            if (token == null || token.ExpiresAt < DateTime.UtcNow)
                throw new AppException("Invalid or expired token", 401);

            await _refreshTokenRepo.InvalidateAsync(token);
            return true;
        }

        private RefreshToken GenerateRefreshToken(Guid userId)
        {
            return new RefreshToken()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
            };
        }

        /// <summary>
        /// Revokes all refresh tokens for a given user, effectively logging them out from all devices.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if all tokens were revoked successfully.</returns>
        public async Task<bool> LogoutAllAsync(Guid userId)
        {
            await _refreshTokenRepo.RevokeAllAsync(userId);
            return true;
        }

    }
}
