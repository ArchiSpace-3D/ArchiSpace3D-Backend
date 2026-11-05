using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using ArchiSpace3D.Api.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    public class UsarInvitacionRequest
    {
        public int IdClienteUsado { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class invitacionController : ControllerBase
    {
        private readonly invitacionServiceImpl _service;
        private readonly proyectoServiceImpl _proyectoService;

        public invitacionController(invitacionServiceImpl service, proyectoServiceImpl proyectoService)
        {
            _service = service;
            _proyectoService = proyectoService;
        }

        private async Task<IActionResult?> ValidarPertenenciaProyectoAsync(int idProyecto)
        {
            var acceso = await _proyectoService.TieneAccesoAsync(idProyecto, User.GetIdUsuario(), User.GetRol());
            if (acceso is null) return NotFound($"El proyecto {idProyecto} no existe.");
            return acceso == false ? Forbid() : null;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var inv = await _service.GetByIdAsync(id);
            if (inv is null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(inv.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            return Ok(inv);
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<IActionResult> GetByProyecto(int idProyecto)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(idProyecto);
            if (noPertenece is not null) return noPertenece;

            return Ok(await _service.GetByProyectoAsync(idProyecto));
        }

        [HttpGet("arquitecto/{idArquitecto}")]
        public async Task<IActionResult> GetByArquitecto(int idArquitecto)
        {
            var idUsuario = User.GetIdUsuario();
            var rol = User.GetRol();

            if (!string.Equals(rol, "Arquitecto", StringComparison.OrdinalIgnoreCase) || idArquitecto != idUsuario)
                return Forbid();

            return Ok(await _service.GetByArquitectoAsync(idArquitecto));
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Invitacion invitacion)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(invitacion.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            if (invitacion.Idarquitecto <= 0)
                invitacion.Idarquitecto = User.GetIdUsuario();

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

        [Authorize(Roles = "Cliente")]
        [HttpPut("codigo/{codigo}/usar")]
        public async Task<IActionResult> Usar(string codigo, [FromBody] UsarInvitacionRequest? request)
        {
            var idCliente = User.GetIdUsuario();
            if (idCliente <= 0 && request != null) idCliente = request.IdClienteUsado;

            try
            {
                var usada = await _service.UsarInvitacionAsync(codigo, idCliente);
                return usada ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var invitacion = await _service.GetByIdAsync(id);
            if (invitacion is null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(invitacion.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            var eliminado = await _service.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}
