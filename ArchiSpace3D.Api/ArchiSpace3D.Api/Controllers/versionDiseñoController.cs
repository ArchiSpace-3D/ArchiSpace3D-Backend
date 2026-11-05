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
    [Route("api/versionDiseno")]
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
            var idUsuario = User.GetIdUsuario();
            var rol = User.GetRol();
            var proyectos = string.Equals(rol, "Arquitecto", StringComparison.OrdinalIgnoreCase)
                ? await _proyectoService.GetByArquitectoAsync(idUsuario)
                : await _proyectoService.GetByClienteAsync(idUsuario);

            var ids = proyectos.Select(p => p.Idproyecto).ToHashSet();
            var todos = await _service.GetAllAsync();
            return Ok(todos.Where(v => ids.Contains(v.Idproyecto)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var version = await _service.GetByIdAsync(id);
            if (version is null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(version.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            return Ok(version);
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
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Versiondiseno version)
        {
            if (id != version.Idversiondiseno) return BadRequest("El id de la URL no coincide con el del body.");

            var existente = await _service.GetByIdAsync(id);
            if (existente is null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(existente.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            version.Idproyecto = existente.Idproyecto;
            var actualizado = await _service.ActualizarAsync(version);
            return actualizado ? NoContent() : NotFound();
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
