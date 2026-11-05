using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DiagController : ControllerBase
    {
        private readonly IConfiguration _config;
        public DiagController(IConfiguration config) { _config = config; }

        [HttpGet]
        public IActionResult Get()
        {
            var conn = _config.GetConnectionString("DefaultConnection") ?? string.Empty;
            var preview = conn.Length > 20 ? conn[..20] + "..." : "(configured)";
            return Ok(new
            {
                ConnString = preview,
                Env = _config["ASPNETCORE_ENVIRONMENT"]
            });
        }
    }
}
