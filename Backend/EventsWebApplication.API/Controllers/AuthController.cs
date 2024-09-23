using EventsWebApplication.Application.DTOs;
using EventsWebApplication.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsWebApplication.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<Guid>> Register([FromBody] RegisterDto registerDto)
        {
            var userId = await _authService.RegisterAsync(registerDto);
            if (userId == Guid.Empty)
            {
                return BadRequest("Register Failed. User with such email already exists.");
            }
            return Ok(userId);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            if (response == null)
            {
                return BadRequest("Invalid login or password.");
            }

            Response.Cookies.Append("jwt", response.AccessToken);
            Response.Cookies.Append("refresh_token", response.RefreshToken);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            var response = await _authService.RefreshTokenAsync(refreshToken);
            Response.Cookies.Append("jwt", response.AccessToken);
            Response.Cookies.Append("refresh_token", response.RefreshToken);
            
            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            await _authService.LogoutAsync(refreshToken);
            
            Response.Cookies.Delete("jwt");
            Response.Cookies.Delete("refresh_token");
            
            return Ok("Logged out successfully");
        }
    }
}
