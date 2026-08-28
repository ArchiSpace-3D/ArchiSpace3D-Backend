using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class notificacionController : ControllerBase
    {
        private readonly notificacionServiceImpl _service;

        public notificacionController(notificacionServiceImpl service)
        {
            _service = service;
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
            return Ok(await _service.GetByProyectoAsync(idProyecto));
        }

        [HttpGet("proyecto/{idProyecto}/no-leidas")]
        public async Task<IActionResult> GetNoLeidasByProyecto(int idProyecto)
        {
            return Ok(await _service.GetNoLeidasByProyectoAsync(idProyecto));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Notificacion notificacion)
        {
            var creada = await _service.CrearAsync(notificacion);
            return CreatedAtAction(nameof(GetById), new { id = creada.Idnotificacion }, creada);
        }

        [HttpPut("{id}/leer")]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            var actualizado = await _service.MarcarComoLeidaAsync(id);
            return actualizado ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}