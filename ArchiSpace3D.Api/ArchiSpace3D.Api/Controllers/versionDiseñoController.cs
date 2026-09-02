using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using ArchiSpace3D.Api.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    public class MarcarActualRequest
    {
        public int IdProyecto { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class versionDiseñoController : ControllerBase
    {
        private readonly versionDiseñoServiceImpl _service;
        private readonly proyectoServiceImpl _proyectoService;

        public versionDiseñoController(versionDiseñoServiceImpl service, proyectoServiceImpl proyectoService)
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
            var noPertenece = await ValidarPertenenciaProyectoAsync(idProyecto);
            if (noPertenece is not null) return noPertenece;

            return Ok(await _service.GetByProyectoAsync(idProyecto));
        }

        [HttpGet("proyecto/{idProyecto}/actual")]
        public async Task<IActionResult> GetVersionActual(int idProyecto)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(idProyecto);
            if (noPertenece is not null) return noPertenece;

            var actual = await _service.GetVersionActualAsync(idProyecto);
            return actual is null ? NotFound() : Ok(actual);
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Versiondiseno version)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(version.Idproyecto);
            if (noPertenece is not null) return noPertenece;

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

        [Authorize(Roles = "Arquitecto")]
        [HttpPut("{id}/actual")]
        public async Task<IActionResult> MarcarComoActual(int id, [FromBody] MarcarActualRequest request)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(request.IdProyecto);
            if (noPertenece is not null) return noPertenece;

            var actualizado = await _service.MarcarComoActualAsync(id, request.IdProyecto);
            return actualizado ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var version = await _service.GetByIdAsync(id);
            if (version is null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(version.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            var eliminado = await _service.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}