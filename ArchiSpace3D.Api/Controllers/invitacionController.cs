using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    public class UsarInvitacionRequest
    {
        public int IdClienteUsado { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class invitacionController : ControllerBase
    {
        private readonly invitacionServiceImpl _service;

        public invitacionController(invitacionServiceImpl service)
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
            var invitacion = await _service.GetByIdAsync(id);
            return invitacion is null ? NotFound() : Ok(invitacion);
        }

        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetByCodigo(string codigo)
        {
            var invitacion = await _service.GetByCodigoAsync(codigo);
            return invitacion is null ? NotFound() : Ok(invitacion);
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<IActionResult> GetByProyecto(int idProyecto)
        {
            return Ok(await _service.GetByProyectoAsync(idProyecto));
        }

        [HttpGet("arquitecto/{idArquitecto}")]
        public async Task<IActionResult> GetByArquitecto(int idArquitecto)
        {
            return Ok(await _service.GetByArquitectoAsync(idArquitecto));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Invitacion invitacion)
        {
            try
            {
                var creada = await _service.CrearAsync(invitacion);
                return CreatedAtAction(nameof(GetById), new { id = creada.Idinvitacion }, creada);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // PUT api/invitacion/codigo/ABC123/usar   body: { "idClienteUsado": 12 }
        [HttpPut("codigo/{codigo}/usar")]
        public async Task<IActionResult> Usar(string codigo, [FromBody] UsarInvitacionRequest request)
        {
            try
            {
                var usada = await _service.UsarInvitacionAsync(codigo, request.IdClienteUsado);
                return usada ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                // Ya usada / expirada / no existe -> el Service decide el motivo exacto
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}