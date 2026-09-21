using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagController : ControllerBase
    {
        private readonly IConfiguration _config;
        public DiagController(IConfiguration config) { _config = config; }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new {
                ConnString = _config.GetConnectionString("DefaultConnection")?.Substring(0, 30) + "...",
                Env = _config["ASPNETCORE_ENVIRONMENT"]
            });
        }
    }
}
