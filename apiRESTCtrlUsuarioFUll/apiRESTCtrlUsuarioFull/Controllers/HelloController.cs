using Microsoft.AspNetCore.Mvc;

namespace apiRESTCtrlUsuarioFull.Controllers   // 
{
    [ApiController]
    [Route("api/[controller]")]               // /api/hello
    public class HelloController : ControllerBase
    {
        [HttpGet("ping")]                     // /api/hello/ping
        public IActionResult Ping() => Ok(new { ok = true, now = DateTime.Now });
    }
}
