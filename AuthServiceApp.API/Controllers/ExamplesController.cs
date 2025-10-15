using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServiceApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamplesController : Controller
    {
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok();
        }

        [HttpGet("authOnly")]
        [Authorize]
        public IActionResult authOnly()
        {
            return Ok();
        }
    }
}
