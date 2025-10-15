using AuthServiceApp.API.Interfaces.Services;
using AuthServiceApp.API.Models.Requets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
        IAuthService authService) : Controller
    {

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRequest request)
        {
            var user = await authService.RegisterAsync(request.Username, request.Password);
            
            if (user is null)
                return BadRequest("User already exist");
            
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserRequest request)
        {
            var tokens = await authService.LoginAsync(request.Username, request.Password);

            if (tokens is null)
                return BadRequest("Invalid username or password");

            return Ok(tokens);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokensRequest request)
        {
            var newTokens = await authService.RefreshTokensAsync(request.AccessToken, request.RefreshToken);

            if (newTokens is null)
                return BadRequest("Invalid refresh token");

            return Ok(newTokens);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var accessToken = Request.Headers["Authorization"]
                .ToString()
                .Substring("Bearer ".Length)
                .Trim();

            var result = await authService.LogoutAsync(accessToken);
            
            if (result)
                return Ok();

            return BadRequest();
        }
    }
}
