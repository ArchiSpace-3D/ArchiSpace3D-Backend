using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    public class MarcarActualRequest
    {
        public int IdProyecto { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class versionDiseñoController : ControllerBase
    {
        private readonly versionDiseñoServiceImpl _service;

        public versionDiseñoController(versionDiseñoServiceImpl service)
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
            var version = await _service.GetByIdAsync(id);
            return version is null ? NotFound() : Ok(version);
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<IActionResult> GetByProyecto(int idProyecto)
        {
            return Ok(await _service.GetByProyectoAsync(idProyecto));
        }

        [HttpGet("proyecto/{idProyecto}/actual")]
        public async Task<IActionResult> GetVersionActual(int idProyecto)
        {
            var actual = await _service.GetVersionActualAsync(idProyecto);
            return actual is null ? NotFound() : Ok(actual);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Versiondiseno version)
        {
            try
            {
                var creada = await _service.CrearAsync(version);
                return CreatedAtAction(nameof(GetById), new { id = creada.Idversiondiseno }, creada);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id}/actual")]
        public async Task<IActionResult> MarcarComoActual(int id, [FromBody] MarcarActualRequest request)
        {
            var actualizado = await _service.MarcarComoActualAsync(id, request.IdProyecto);
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