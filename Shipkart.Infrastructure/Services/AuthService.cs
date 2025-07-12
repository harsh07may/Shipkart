using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

        public AuthService(
            IUserRepository userRepository,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepo)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new AppException("Invalid credentials", 401);

            var accessToken = _tokenService.GenerateToken(user);

            var refreshToken = GenerateRefreshToken(user.Id);

            await _refreshTokenRepo.AddAsync(refreshToken);

            return new AuthResponseDto
            {
                Email = user.Email,
                Token = accessToken,
                RefreshToken = refreshToken.Token,
            };
        }

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

        public async Task<bool> LogoutAllAsync(Guid userId)
        {
            await _refreshTokenRepo.RevokeAllAsync(userId);
            return true;
        }

    }
}
