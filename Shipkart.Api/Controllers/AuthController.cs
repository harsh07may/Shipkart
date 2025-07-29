using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipkart.Application.Common;
using Shipkart.Application.DTOs.Users;
using Shipkart.Application.Interfaces;
using Shipkart.Infrastructure.Services;

namespace Shipkart.Api.Controllers
{
    /// <summary>
    /// Controller for handling authentication-related operations such as login, logout, token refresh, and password management.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;
        private readonly IPasswordResetService _passwordResetService;

        public AuthController(IAuthService authService, IPasswordResetService passwordResetService)
        {
            _authService = authService;
            _passwordResetService = passwordResetService;
        }

        /// <summary>
        /// Retrieves the details of the currently authenticated user.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the user's email and name.</returns>
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {

            var email = User.FindFirstValue(ClaimTypes.Email);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var name = User.Identity?.Name;

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                Email = email,
                Name = name,
                Role = role
            }));
        }

        /// <summary>
        /// Authenticates a user and provides an access token and refresh token.
        /// </summary>
        /// <param name="dto">The user login data.</param>
        /// <returns>An <see cref="IActionResult"/> containing the access token and user email, and sets a refresh token cookie.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                result.Email,
                result.Name,
                result.Role,
                result.Token
            }));
        }

        /// <summary>
        /// Refreshes the access token using a refresh token provided in the cookie.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing a new access token, and sets a new refresh token cookie.</returns>
        /// <response code="401">If no refresh token is found in the cookies.</response>
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

        /// <summary>
        /// Logs out the current user by deleting the refresh token cookie.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> indicating successful logout.</returns>
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

        /// <summary>
        /// Initiates the forgot password process by sending a reset link to the user's email.
        /// </summary>
        /// <param name="dto">The data transfer object containing the user's email.</param>
        /// <returns>An <see cref="IActionResult"/> indicating that reset instructions have been sent (if the email exists).</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            await _passwordResetService.RequestResetAsync(dto.Email);
            return Ok(ApiResponse<string>.SuccessResponse("If your email exists, reset instructions have been sent."));
        }

        /// <summary>
        /// Resets the user's password using a valid reset token.
        /// </summary>
        /// <param name="dto">The data transfer object containing the reset token and new password.</param>
        /// <returns>An <see cref="IActionResult"/> indicating successful password reset.</returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            await _passwordResetService.ResetPasswordAsync(dto.Token, dto.NewPassword);
            return Ok(ApiResponse<string>.SuccessResponse("Password reset successful."));
        }

        /// <summary>
        /// Logs out the current user by invalidate all refresh tokens.
        /// </summary>
        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Unauthorized(ApiResponse<string>.Failure("Invalid user"));

            await _authService.LogoutAllAsync(Guid.Parse(userId));

            Response.Cookies.Delete("refreshToken");

            return Ok(ApiResponse<string>.SuccessResponse("Logged out from all sessions"));
        }


        /// <summary>
        /// Sets the refresh token as an HTTP-only cookie in the response.
        /// </summary>
        /// <param name="token">The refresh token string.</param>
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
