using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipkart.Application.Common;
using Shipkart.Application.DTOs.Users;
using Shipkart.Application.Interfaces;
using System.Security.Claims;

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

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userName = User.Identity?.Name;
            return Ok(new { Email = userEmail, Name = userName });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userName = User.Identity?.Name;
            return Ok($"Hi {userName}, You are an Admin.");
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(result, "User registered successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful"));
        }


    }
}
