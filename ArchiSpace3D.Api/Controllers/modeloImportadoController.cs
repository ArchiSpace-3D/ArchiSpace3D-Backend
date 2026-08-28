using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class modeloImportadoController : ControllerBase
    {
        private readonly modeloImportadoServiceImpl _service;

        public modeloImportadoController(modeloImportadoServiceImpl service)
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
            var modelo = await _service.GetByIdAsync(id);
            return modelo is null ? NotFound() : Ok(modelo);
        }

        [HttpGet("version/{idVersionDiseno}")]
        public async Task<IActionResult> GetByVersionDiseno(int idVersionDiseno)
        {
            return Ok(await _service.GetByVersionDisenoAsync(idVersionDiseno));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Modeloimportado modelo)
        {
            var creado = await _service.CrearAsync(modelo);
            return CreatedAtAction(nameof(GetById), new { id = creado.Idmodeloimportado }, creado);
        }

        [HttpPut("{id}/transform")]
        public async Task<IActionResult> ActualizarTransform(int id, [FromBody] Modeloimportado modelo)
        {
            if (id != modelo.Idmodeloimportado)
            {
                return BadRequest("El id de la URL no coincide con el del body.");
            }

            var actualizado = await _service.ActualizarTransformAsync(modelo);
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