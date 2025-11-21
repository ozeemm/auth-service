using AuthServiceApp.API.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
        IUserService userService) : Controller
    {
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await userService.GetUsers();
            return Ok(users);
        }
    }
}
