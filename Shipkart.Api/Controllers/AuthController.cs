using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipkart.Application.Common;
using Shipkart.Application.DTOs.Users;
using Shipkart.Application.Interfaces;

namespace Shipkart.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                result.Email,
                result.Token
            }));
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var token = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized(ApiResponse<string>.Failure("No refresh token found"));

            var result = await _authService.RefreshTokenAsync(token);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                Token = result.AccessToken
            }));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(token))
            {
                await _authService.LogoutAsync(token);
                Response.Cookies.Delete("refreshToken");
            }

            return Ok(ApiResponse<string>.SuccessResponse("Logged out"));
        }

        // TODO: Do this later.
        //[Authorize]
        //[HttpPost("logout-all")]
        //public async Task<IActionResult> LogoutAll()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (userId is null)
        //        return Unauthorized(ApiResponse<string>.Failure("Invalid user"));

        //    await _authService.LogoutAllAsync(Guid.Parse(userId));

        //    Response.Cookies.Delete("refreshToken");

        //    return Ok(ApiResponse<string>.SuccessResponse("Logged out from all sessions"));
        //}


        private void SetRefreshTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7),
                Path = "/"
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

    }
}
