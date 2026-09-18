using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Services;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicionController : ControllerBase
    {
        private readonly IMedicionService _service;
        private readonly IProyectoService _proyectoService;

        public MedicionController(IMedicionService service, IProyectoService proyectoService)
        {
            _service = service;
            _proyectoService = proyectoService;
        }

        private async Task<IActionResult?> ValidarPertenenciaProyectoAsync(int idProyecto)
        {
            var proyecto = await _proyectoService.GetByIdAsync(idProyecto);
            if (proyecto == null) return NotFound("Proyecto no encontrado");

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            if (proyecto.Idarquitecto != userId)
            {
                var invitado = await _proyectoService.VerificarInvitadoAsync(idProyecto, userId);
                if (!invitado) return Forbid();
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Medicion medicion)
        {
            try
            {
                var noPertenece = await ValidarPertenenciaProyectoAsync(medicion.Idproyecto);
                if (noPertenece is not null) return noPertenece;

                var creada = await _service.CreateAsync(medicion);
                return CreatedAtAction(nameof(GetById), new { id = creada.Idmedicion }, creada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Error: " + ex.Message + "\nInner: " + ex.InnerException?.Message + "\nTrace: " + ex.StackTrace);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var medicion = await _service.GetByIdAsync(id);
            if (medicion == null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(medicion.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            return Ok(medicion);
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<IActionResult> GetByProyecto(int idProyecto)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(idProyecto);
            if (noPertenece is not null) return noPertenece;

            return Ok(await _service.GetByProyectoAsync(idProyecto));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var medicion = await _service.GetByIdAsync(id);
            if (medicion == null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(medicion.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
