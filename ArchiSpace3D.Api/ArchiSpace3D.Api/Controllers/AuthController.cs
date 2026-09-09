using ArchiSpace3D.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Contrasena { get; set; } = null!;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthServiceImpl _authService;

        public AuthController(AuthServiceImpl authService)
        {
            _authService = authService;
        }

        // [AllowAnonymous]: este es el único endpoint que debe funcionar SIN
        // token todavía -- es justamente el que entrega el token.
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var (token, usuario) = await _authService.LoginAsync(request.Email, request.Contrasena);

                return Ok(new
                {
                    token,
                    usuario.Idusuario,
                    usuario.Nombre,
                    usuario.Apellido,
                    usuario.Email,
                    usuario.Rol
                });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(ex.Message); // 401
            }
        }
    }
}