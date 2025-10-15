using AuthServiceApp.API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(
        IUserService userService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await userService.GetUsers();
            return Ok(users);
        }
    }
}
