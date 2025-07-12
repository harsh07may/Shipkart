using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipkart.Application.Common;
using Shipkart.Application.DTOs.Users;
using Shipkart.Application.Interfaces;
using System.Security.Claims;

/// <summary>
/// Controller for managing user-related operations, including registration and fetching user details.
/// </summary>
namespace Shipkart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Retrieves the details of the currently authenticated user.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the user's email and name.</returns>
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userName = User.Identity?.Name;
            return Ok(new { Email = userEmail, Name = userName });
        }

        // <summary>
        // Dummy endpoint for testing admin role authorization.
        // </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userName = User.Identity?.Name;
            return Ok(ApiResponse<string>.SuccessResponse($"Hi {userName}, You are an Admin."));
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="dto">The data transfer object containing user registration details.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the success of the registration.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(result, "User registered successfully"));
        }

    }
}
