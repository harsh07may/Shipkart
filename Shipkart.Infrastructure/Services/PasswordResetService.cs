using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Shipkart.Application.Common.Constants;
using Shipkart.Application.Exceptions;
using Shipkart.Application.Interfaces;
using Shipkart.Domain.Entities;

namespace Shipkart.Infrastructure.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordResetTokenRepository _tokenRepo;
        private readonly IEmailService _emailService;

        public PasswordResetService(
            IUserRepository userRepo,
            IPasswordResetTokenRepository tokenRepo,
            IEmailService emailService)
        {
            _userRepo = userRepo;
            _tokenRepo = tokenRepo;
            _emailService = emailService;
        }

        public async Task RequestResetAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
                return; // Silently ignore for security

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Constants.PasswordResetTokenExpiryMinutes)
            };

            await _tokenRepo.AddAsync(resetToken);

            var resetLink = $"https://localhost:3000/reset-password?token={Uri.EscapeDataString(token)}";

            var body = $"""
            <p>Hello {user.FirstName},</p>
            <p>Click the link below to reset your password:</p>
            <a href="{resetLink}">Reset Password</a>
            <p>This link is valid for 30 minutes.</p>
            """;

            await _emailService.SendEmailAsync(user.Email, Constants.PasswordResetSubject, body);
        }



        public async Task ResetPasswordAsync(string token, string newPassword)
        {
            var tokenRecord = await _tokenRepo.GetByTokenAsync(token) ?? throw new AppException("Invalid or expired token", 400);
            var user = tokenRecord.User!;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            
            await _tokenRepo.InvalidateTokenAsync(tokenRecord.Id);
        }

    }
}
