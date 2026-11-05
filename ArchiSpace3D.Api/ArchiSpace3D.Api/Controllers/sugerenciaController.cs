using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using ArchiSpace3D.Api.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SugerenciaController : ControllerBase
    {
        private readonly ArchiSpaceContext _context;
        private readonly notificacionServiceImpl _notificacionService;

        public SugerenciaController(ArchiSpaceContext context, notificacionServiceImpl notificacionService)
        {
            _context = context;
            _notificacionService = notificacionService;
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<ActionResult<IEnumerable<Sugerencia>>> GetSugerenciasByProyecto(int idProyecto)
        {
            return await _context.Sugerencias
                .AsNoTracking()
                .Where(s => s.Idproyecto == idProyecto)
                .OrderByDescending(s => s.Fechacreacion)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Sugerencia>> PostSugerencia(Sugerencia sugerencia)
        {
            if (sugerencia.Idusuario <= 0)
            {
                sugerencia.Idusuario = User.GetIdUsuario();
            }

            sugerencia.Fechacreacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            sugerencia.Estado = string.IsNullOrWhiteSpace(sugerencia.Estado) ? "Pendiente" : sugerencia.Estado;

            _context.Sugerencias.Add(sugerencia);
            await _context.SaveChangesAsync();

            try
            {
                await _notificacionService.NotificarCambioAsync(
                    sugerencia.Idproyecto,
                    "nueva_sugerencia",
                    $"Nueva sugerencia registrada: {sugerencia.Titulo}");
            }
            catch { }

            return CreatedAtAction(nameof(GetSugerenciasByProyecto), new { idProyecto = sugerencia.Idproyecto }, sugerencia);
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPut("{id}/estado")]
        public async Task<IActionResult> UpdateEstado(int id, [FromBody] string estado)
        {
            var sugerencia = await _context.Sugerencias.FindAsync(id);
            if (sugerencia == null) return NotFound();

            sugerencia.Estado = estado;
            await _context.SaveChangesAsync();

            try
            {
                await _notificacionService.NotificarCambioAsync(
                    sugerencia.Idproyecto,
                    "estado_sugerencia",
                    $"La sugerencia '{sugerencia.Titulo}' cambió a estado: {estado}.");
            }
            catch { }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSugerencia(int id)
        {
            var sugerencia = await _context.Sugerencias.FindAsync(id);
            if (sugerencia == null) return NotFound();

            if (!User.IsInRole("Arquitecto") && sugerencia.Idusuario != User.GetIdUsuario())
            {
                return Forbid();
            }

            _context.Sugerencias.Remove(sugerencia);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
