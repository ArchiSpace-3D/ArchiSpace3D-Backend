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
    public class notificacionController : ControllerBase
    {
        private readonly notificacionServiceImpl _service;
        private readonly proyectoServiceImpl _proyectoService;

        public notificacionController(notificacionServiceImpl service, proyectoServiceImpl proyectoService)
        {
            _service = service;
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
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var notificacion = await _service.GetByIdAsync(id);
            return notificacion is null ? NotFound() : Ok(notificacion);
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<IActionResult> GetByProyecto(int idProyecto)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(idProyecto);
            if (noPertenece is not null) return noPertenece;

            return Ok(await _service.GetByProyectoAsync(idProyecto));
        }

        [HttpGet("proyecto/{idProyecto}/no-leidas")]
        public async Task<IActionResult> GetNoLeidasByProyecto(int idProyecto)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(idProyecto);
            if (noPertenece is not null) return noPertenece;

            return Ok(await _service.GetNoLeidasByProyectoAsync(idProyecto));
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Notificacion notificacion)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(notificacion.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            var creada = await _service.CrearAsync(notificacion);
            return CreatedAtAction(nameof(GetById), new { id = creada.Idnotificacion }, creada);
        }

        [HttpPut("{id}/leer")]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            var notificacion = await _service.GetByIdAsync(id);
            if (notificacion is null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(notificacion.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            var actualizado = await _service.MarcarComoLeidaAsync(id);
            return actualizado ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var notificacion = await _service.GetByIdAsync(id);
            if (notificacion is null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(notificacion.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            var eliminado = await _service.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}