using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using FluentAssertions;
using Shipkart.Application.DTOs.Users;
using Shipkart.Application.Exceptions;
using Shipkart.Application.Interfaces;
using Shipkart.Domain.Entities;
using Shipkart.Infrastructure.Services;
using Shipkart.Domain.Enums;

namespace Shipkart.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ILoginThrottlingService> _loginThrottlingMock;

        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
            _tokenServiceMock = new Mock<ITokenService>();
            _loginThrottlingMock = new Mock<ILoginThrottlingService>();

            _sut = new AuthService(
                _userRepoMock.Object,
                _tokenServiceMock.Object,
                _refreshTokenRepoMock.Object,
                _loginThrottlingMock.Object
            );
        }

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsAuthResponseDto()
        {
            // Arrange
            var dto = new UserLoginDto
            {
                Email = "valid@example.com",
                Password = "secure123"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = "Alice",
                LastName = "Smith",
                Role = UserRole.Admin
            };

            _loginThrottlingMock
                .Setup(s => s.IsLockedOutAsync(dto.Email))
                .ReturnsAsync(false);

            _userRepoMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            _tokenServiceMock
                .Setup(t => t.GenerateToken(user))
                .Returns("access-token");

            _refreshTokenRepoMock
                .Setup(r => r.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _loginThrottlingMock
                .Setup(s => s.ResetAttemptsAsync(dto.Email))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.LoginAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(dto.Email);
            result.Token.Should().Be("access-token");
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();

            _refreshTokenRepoMock.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
            _loginThrottlingMock.Verify(s => s.ResetAttemptsAsync(dto.Email), Times.Once);
        }


        [Fact]
        public async Task LoginAsync_InvalidCredentials_ThrowsAppExceptionAndRegistersAttempt()
        {
            // Arrange
            var dto = new UserLoginDto
            {
                Email = "user@example.com",
                Password = "wrong-password"
            };

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password"),
                FirstName = "John",
                LastName = "Doe",
                Role = UserRole.Customer
            };

            _loginThrottlingMock
                .Setup(s => s.IsLockedOutAsync(dto.Email))
                .ReturnsAsync(false);

            _userRepoMock
                .Setup(r => r.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            // Act
            var act = async () => await _sut.LoginAsync(dto);

            // Assert
            var ex = await Assert.ThrowsAsync<AppException>(act);
            ex.Message.Should().Be("Invalid credentials");
            ex.StatusCode.Should().Be(401);

            _loginThrottlingMock.Verify(s => s.RegisterFailedAttemptAsync(dto.Email), Times.Once);
            _refreshTokenRepoMock.Verify(x => x.AddAsync(It.IsAny<RefreshToken>()), Times.Never);
            _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }


        [Fact]
        public async Task LoginAsync_UserIsLockedOut_ThrowsAppException()
        {
            // Arrange
            var dto = new UserLoginDto
            {
                Email = "locked@example.com",
                Password = "any-password"
            };

            _loginThrottlingMock
                .Setup(s => s.IsLockedOutAsync(dto.Email))
                .ReturnsAsync(true);

            // Act
            var act = async () => await _sut.LoginAsync(dto);

            // Assert
            var ex = await Assert.ThrowsAsync<AppException>(act);
            ex.Message.Should().Be("Too many failed login attempts. Try again later.");
            ex.StatusCode.Should().Be(429);

            _userRepoMock.Verify(x => x.GetByEmailAsync(It.IsAny<string>()), Times.Never);
            _tokenServiceMock.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        #endregion

        #region LogoutAsync Tests

        [Fact]
        public async Task LogoutAsync_ValidToken_RevokesToken()
        {
            // Arrange
            var tokenValue = "valid-refresh-token";
            var tokenEntity = new RefreshToken
            {
                Token = tokenValue,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            _refreshTokenRepoMock
                .Setup(r => r.GetByTokenAsync(tokenValue))
                .ReturnsAsync(tokenEntity);

            _refreshTokenRepoMock
                .Setup(r => r.InvalidateAsync(tokenEntity))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.LogoutAsync(tokenValue);

            // Assert
            result.Should().BeTrue();
            _refreshTokenRepoMock.Verify(r => r.InvalidateAsync(tokenEntity), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("expired-token")]
        public async Task LogoutAsync_InvalidOrExpiredToken_ThrowsAppException(string? tokenValue)
        {
            // Arrange
            RefreshToken? tokenEntity = tokenValue switch
            {
                null => null,
                "expired-token" => new RefreshToken
                {
                    Token = tokenValue,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(-5)
                },
                _ => throw new ArgumentOutOfRangeException()
            };

            _refreshTokenRepoMock
                .Setup(r => r.GetByTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(tokenEntity);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _sut.LogoutAsync(tokenValue ?? "token"));
            ex.StatusCode.Should().Be(401);
            ex.Message.Should().Be("Invalid or expired token");
        }

        #endregion

        #region RefreshTokenAsync Tests

        [Fact]
        public async Task RefreshTokenAsync_ValidToken_ReturnsNewTokens()
        {
            // Arrange
            var oldTokenValue = "valid-refresh-token";
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = UserRole.Customer
            };

            var oldToken = new RefreshToken
            {
                Token = oldTokenValue,
                User = user,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            _refreshTokenRepoMock
                .Setup(r => r.GetByTokenAsync(oldTokenValue))
                .ReturnsAsync(oldToken);

            _tokenServiceMock
                .Setup(t => t.GenerateToken(user))
                .Returns("new-access-token");

            _refreshTokenRepoMock
                .Setup(r => r.AddAsync(It.IsAny<RefreshToken>()))
                .Returns(Task.CompletedTask);

            _refreshTokenRepoMock
                .Setup(r => r.InvalidateAsync(oldToken))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.RefreshTokenAsync(oldTokenValue);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be("new-access-token");
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();

            _refreshTokenRepoMock.Verify(r => r.InvalidateAsync(oldToken), Times.Once);
            _refreshTokenRepoMock.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_TokenNotFound_ThrowsAppException()
        {
            // Arrange
            var token = "nonexistent";
            _refreshTokenRepoMock
                .Setup(r => r.GetByTokenAsync(token))
                .ReturnsAsync((RefreshToken?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _sut.RefreshTokenAsync(token));
            ex.StatusCode.Should().Be(401);
            ex.Message.Should().Be("Invalid or expired refresh token");
        }

        [Fact]
        public async Task RefreshTokenAsync_TokenExpired_ThrowsAppException()
        {
            // Arrange
            var tokenValue = "expired-token";
            var expiredToken = new RefreshToken
            {
                Token = tokenValue,
                ExpiresAt = DateTime.UtcNow.AddMinutes(-5)
            };

            _refreshTokenRepoMock
                .Setup(r => r.GetByTokenAsync(tokenValue))
                .ReturnsAsync(expiredToken);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<AppException>(() => _sut.RefreshTokenAsync(tokenValue));
            ex.StatusCode.Should().Be(401);
            ex.Message.Should().Be("Invalid or expired refresh token");
        }

        #endregion

        #region LogoutAllAsync Tests

        [Fact]
        public async Task LogoutAllAsync_ValidUserId_RevokesAllTokens()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _refreshTokenRepoMock
                .Setup(r => r.RevokeAllAsync(userId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _sut.LogoutAllAsync(userId);

            // Assert
            result.Should().BeTrue();
            _refreshTokenRepoMock.Verify(r => r.RevokeAllAsync(userId), Times.Once);
        }


        #endregion
    }
}