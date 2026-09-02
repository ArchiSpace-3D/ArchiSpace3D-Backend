using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using ArchiSpace3D.Api.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class usuarioController : ControllerBase
    {
        private readonly usuarioServiceImpl _usuarioService;

        public usuarioController(usuarioServiceImpl usuarioService)
        {
            _usuarioService = usuarioService;
        }

       
        private IActionResult? ValidarPropiaCuenta(int idUsuarioObjetivo)
        {
            var idUsuario = User.GetIdUsuario();
            return idUsuarioObjetivo == idUsuario ? null : Forbid();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _usuarioService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var noPermitido = ValidarPropiaCuenta(id);
            if (noPermitido is not null) return noPermitido;

            var usuario = await _usuarioService.GetByIdAsync(id);
            return usuario is null ? NotFound() : Ok(usuario);
        }

        [AllowAnonymous]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Idusuario)
                return BadRequest("El id de la URL no coincide con el del body.");

            var noPermitido = ValidarPropiaCuenta(id);
            if (noPermitido is not null) return noPermitido;

            var actualizado = await _usuarioService.ActualizarAsync(usuario);
            return actualizado ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var noPermitido = ValidarPropiaCuenta(id);
            if (noPermitido is not null) return noPermitido;

            var eliminado = await _usuarioService.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}