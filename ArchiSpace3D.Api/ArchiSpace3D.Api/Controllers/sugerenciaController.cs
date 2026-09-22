using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SugerenciaController : ControllerBase
    {
        private readonly ArchiSpaceContext _context;

        public SugerenciaController(ArchiSpaceContext context)
        {
            _context = context;
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<ActionResult<IEnumerable<Sugerencia>>> GetSugerenciasByProyecto(int idProyecto)
        {
            return await _context.Sugerencias
                .Where(s => s.Idproyecto == idProyecto)
                .OrderByDescending(s => s.Fechacreacion)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Sugerencia>> PostSugerencia(Sugerencia sugerencia)
        {
            sugerencia.Fechacreacion = DateTime.UtcNow;
            sugerencia.Estado = "Pendiente";
            
            _context.Sugerencias.Add(sugerencia);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSugerenciasByProyecto), new { idProyecto = sugerencia.Idproyecto }, sugerencia);
        }

        [HttpPut("{id}/estado")]
        public async Task<IActionResult> UpdateEstado(int id, [FromBody] string estado)
        {
            var sugerencia = await _context.Sugerencias.FindAsync(id);
            if (sugerencia == null) return NotFound();

            sugerencia.Estado = estado;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSugerencia(int id)
        {
            var sugerencia = await _context.Sugerencias.FindAsync(id);
            if (sugerencia == null) return NotFound();

            _context.Sugerencias.Remove(sugerencia);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
