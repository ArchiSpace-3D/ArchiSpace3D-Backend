using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;
using Microsoft.AspNetCore.Mvc;
using ArchiSpace3D.Api.Service;

namespace ArchiSpace3D.Api.Controllers
{
    // TEMPORAL: inyecta el DAO directo. Cambiar a IMedicionService cuando exista.
    // Nota: no hay endpoint PUT porque MedicionDao no tiene UpdateAsync
    // (una medición capturada no se edita, se borra y se vuelve a crear).
    [ApiController]
    [Route("api/[controller]")]
    public class MedicionController : ControllerBase
    {
        private readonly medicionService _service;

        public MedicionController(medicionService service)
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
            var medicion = await _service.GetByIdAsync(id);
            return medicion is null ? NotFound() : Ok(medicion);
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<IActionResult> GetByProyecto(int idProyecto)
        {
            return Ok(await _service.GetByProyectoAsync(idProyecto));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Medicion medicion)
        {
            var creada = await _service.CreateAsync(medicion);
            return CreatedAtAction(nameof(GetById), new { id = creada.Idmedicion }, creada);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _service.DeleteAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}