using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class elementoEstructuralController : ControllerBase
    {
        private readonly elementoEstructuralServiceImpl _service;

        public elementoEstructuralController(elementoEstructuralServiceImpl service)
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
            var elemento = await _service.GetByIdAsync(id);
            return elemento is null ? NotFound() : Ok(elemento);
        }

        [HttpGet("version/{idVersionDiseno}")]
        public async Task<IActionResult> GetByVersionDiseno(int idVersionDiseno)
        {
            return Ok(await _service.GetByVersionDisenoAsync(idVersionDiseno));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Elementoestructural elemento)
        {
            var creado = await _service.CrearAsync(elemento);
            return CreatedAtAction(nameof(GetById), new { id = creado.Idelementoestructural }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Elementoestructural elemento)
        {
            if (id != elemento.Idelementoestructural)
            {
                return BadRequest("El id de la URL no coincide con el del body.");
            }

            var actualizado = await _service.ActualizarAsync(elemento);
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