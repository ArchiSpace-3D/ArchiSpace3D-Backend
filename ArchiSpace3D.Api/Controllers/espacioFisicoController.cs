using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class espacioFisicoController : ControllerBase
    {
        private readonly espacioFisicoServiceImpl _service;

        public espacioFisicoController(espacioFisicoServiceImpl service)
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
            var espacio = await _service.GetByIdAsync(id);
            return espacio is null ? NotFound() : Ok(espacio);
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<IActionResult> GetByProyecto(int idProyecto)
        {
            var espacio = await _service.GetByProyectoAsync(idProyecto);
            return espacio is null ? NotFound() : Ok(espacio);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Espaciofisico espacio)
        {
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Espaciofisico espacio)
        {
            if (id != espacio.Idespaciofisico)
            {
                return BadRequest("El id de la URL no coincide con el del body.");
            }

            var actualizado = await _service.ActualizarAsync(espacio);
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