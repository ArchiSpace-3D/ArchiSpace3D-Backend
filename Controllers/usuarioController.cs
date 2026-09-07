using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class usuarioController : ControllerBase
    {
        private readonly usuarioServiceImpl _usuarioService;

        public usuarioController(usuarioServiceImpl usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _usuarioService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            return usuario is null ? NotFound() : Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
        {
            try
            {
                var creado = await _usuarioService.RegistrarAsync(usuario);
                return CreatedAtAction(nameof(GetById), new { id = creado.Idusuario }, creado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _usuarioService.LoginAsync(request.Email, request.Password);
            if (usuario == null)
            {
                return Unauthorized("Credenciales inválidas.");
            }
            
            // NOTE: Here you would normally generate a JWT token.
            // For now, we return the user object (excluding the hash).
            usuario.Contrasena = string.Empty;
            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Idusuario)
            {
                return BadRequest("El id de la URL no coincide con el del body.");
            }

            var actualizado = await _usuarioService.ActualizarAsync(usuario);
            return actualizado ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _usuarioService.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}