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
    public class espacioFisicoController : ControllerBase
    {
        private readonly espacioFisicoServiceImpl _service;
        private readonly proyectoServiceImpl _proyectoService;

        public espacioFisicoController(espacioFisicoServiceImpl service, proyectoServiceImpl proyectoService)
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
            var espacio = await _service.GetByIdAsync(id);
            return espacio is null ? NotFound() : Ok(espacio);
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<IActionResult> GetByProyecto(int idProyecto)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(idProyecto);
            if (noPertenece is not null) return noPertenece;

            var espacio = await _service.GetByProyectoAsync(idProyecto);
            return espacio is null ? NotFound() : Ok(espacio);
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Espaciofisico espacio)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(espacio.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            try
            {
                var creado = await _service.CrearAsync(espacio);
                return CreatedAtAction(nameof(GetById), new { id = creado.Idespaciofisico }, creado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Espaciofisico espacio)
        {
            if (id != espacio.Idespaciofisico) return BadRequest("El id de la URL no coincide con el del body.");

            var noPertenece = await ValidarPertenenciaProyectoAsync(espacio.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            var actualizado = await _service.ActualizarAsync(espacio);
            return actualizado ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var espacio = await _service.GetByIdAsync(id);
            if (espacio is null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(espacio.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            var eliminado = await _service.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}