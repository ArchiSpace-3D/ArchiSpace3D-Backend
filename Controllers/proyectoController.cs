using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class proyectoController : ControllerBase
    {
        private readonly proyectoServiceImpl _proyectoService;

        public proyectoController(proyectoServiceImpl proyectoService)
        {
            _proyectoService = proyectoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _proyectoService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            return proyecto is null ? NotFound() : Ok(proyecto);
        }

        [HttpGet("arquitecto/{idArquitecto}")]
        public async Task<IActionResult> GetByArquitecto(int idArquitecto)
        {
            return Ok(await _proyectoService.GetByArquitectoAsync(idArquitecto));
        }

        [HttpGet("cliente/{idCliente}")]
        public async Task<IActionResult> GetByCliente(int idCliente)
        {
            return Ok(await _proyectoService.GetByClienteAsync(idCliente));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Proyecto proyecto)
        {
            try
            {
                var creado = await _proyectoService.CrearAsync(proyecto);
                return CreatedAtAction(nameof(GetById), new { id = creado.Idproyecto }, creado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Proyecto proyecto)
        {
            if (id != proyecto.Idproyecto)
            {
                return BadRequest("El id de la URL no coincide con el del body.");
            }

            var actualizado = await _proyectoService.ActualizarAsync(proyecto);
            return actualizado ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _proyectoService.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}