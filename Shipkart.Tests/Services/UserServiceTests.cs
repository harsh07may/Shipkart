using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;
using Moq;
using FluentAssertions;
using Shipkart.Application.DTOs.Users;
using Shipkart.Application.Interfaces;
using Shipkart.Domain.Entities;
using Shipkart.Infrastructure.Services;
using Shipkart.Application.Exceptions;

namespace Shipkart.Tests.Services
{
    public class UserServiceTests
    {

        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IConfiguration> _configMock;

        private readonly UserService _sut; // system under test

        public UserServiceTests()
        {
            _userRepoMock = new();
            _tokenServiceMock = new();
            _configMock = new();

            _sut = new UserService(
                _userRepoMock.Object,
                _configMock.Object,
                _tokenServiceMock.Object
            );
        }


        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsNewRegisteredUser()
        {
            // Arrange
            var dto = new UserRegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password123",
                Role = Domain.Enums.UserRole.Customer
            };

            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                         .ReturnsAsync((User?)null);

            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                         .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.RegisterAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(dto.Email);
            result.FullName.Should().Be("John Doe");

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_EmailAlreadyExists_ThrowsAppException()
        {
            // Arrange
            var dto = new UserRegisterDto
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@example.com",
                Password = "password123",
                Role = Domain.Enums.UserRole.Customer
            };

            var existingUser = new User { Email = dto.Email };

            _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
                         .ReturnsAsync(existingUser);

            // Act
            var act = async () => await _sut.RegisterAsync(dto);

            // Assert
            var ex = await Assert.ThrowsAsync<AppException>(act);
            ex.Message.Should().Be("Email already registered");

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        }

    }
}
