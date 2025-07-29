using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shipkart.Application.DTOs.Users;
using Shipkart.Application.Exceptions;
using Shipkart.Application.Interfaces;
using Shipkart.Domain.Entities;

namespace Shipkart.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        public UserService(IUserRepository userRepository, IConfiguration config, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _config = config;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="dto">The user registration data transfer object.</param>
        /// <returns>A data transfer object containing the newly registered user's information.</returns>
        public async Task<UserResponseDto> RegisterAsync(UserRegisterDto dto)
        {
            var exists = await _userRepository.GetByEmailAsync(dto.Email);
            if (exists != null)
                throw new AppException("Email already registered", 400);

            var hashedPassword = HashPassword(dto.Password);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = dto.Role

            };

            await _userRepository.AddAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                Name = $"{user.FirstName} {user.LastName}",
                Email = user.Email
            };
        }


        private static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    }
}
