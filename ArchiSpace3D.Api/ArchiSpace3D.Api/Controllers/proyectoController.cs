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
    public class proyectoController : ControllerBase
    {
        private readonly proyectoServiceImpl _proyectoService;

        public proyectoController(proyectoServiceImpl proyectoService)
        {
            _proyectoService = proyectoService;
        }

   
        private async Task<IActionResult?> ValidarPertenenciaProyectoAsync(int idProyecto)
        {
            var acceso = await _proyectoService.TieneAccesoAsync(idProyecto, User.GetIdUsuario(), User.GetRol());
            if (acceso is null) return NotFound($"El proyecto {idProyecto} no existe.");
            return acceso == false ? Forbid() : null;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var idUsuario = User.GetIdUsuario();
            var rol = User.GetRol();

            return rol == "Arquitecto"
                ? Ok(await _proyectoService.GetByArquitectoAsync(idUsuario))
                : Ok(await _proyectoService.GetByClienteAsync(idUsuario));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(id);
            if (noPertenece is not null) return noPertenece;

            var proyecto = await _proyectoService.GetByIdAsync(id);
            return proyecto is null ? NotFound() : Ok(proyecto);
        }

        [HttpGet("arquitecto/{idArquitecto}")]
        public async Task<IActionResult> GetByArquitecto(int idArquitecto)
        {
            var idUsuario = User.GetIdUsuario();
            var rol = User.GetRol();

            if (rol != "Arquitecto" || idArquitecto != idUsuario) return Forbid();

            return Ok(await _proyectoService.GetByArquitectoAsync(idArquitecto));
        }

        [HttpGet("cliente/{idCliente}")]
        public async Task<IActionResult> GetByCliente(int idCliente)
        {
            var idUsuario = User.GetIdUsuario();
            var rol = User.GetRol();

            if (rol != "Cliente" || idCliente != idUsuario) return Forbid();

            return Ok(await _proyectoService.GetByClienteAsync(idCliente));
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Proyecto proyecto)
        {
            try
            {
                var creado = await _proyectoService.CrearAsync(proyecto);
                return CreatedAtAction(nameof(GetById), new { id = creado.Idproyecto }, creado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Proyecto proyecto)
        {
            if (id != proyecto.Idproyecto) return BadRequest("El id de la URL no coincide con el del body.");

            var noPertenece = await ValidarPertenenciaProyectoAsync(id);
            if (noPertenece is not null) return noPertenece;

            var actualizado = await _proyectoService.ActualizarAsync(proyecto);
            return actualizado ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(id);
            if (noPertenece is not null) return noPertenece;

            var eliminado = await _proyectoService.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}