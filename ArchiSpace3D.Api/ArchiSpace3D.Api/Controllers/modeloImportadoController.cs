using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using ArchiSpace3D.Api.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class modeloImportadoController : ControllerBase
    {
        private readonly modeloImportadoServiceImpl _service;
        private readonly versionDiseñoServiceImpl _versionService;
        private readonly proyectoServiceImpl _proyectoService;

        public modeloImportadoController(
            modeloImportadoServiceImpl service,
            versionDiseñoServiceImpl versionService,
            proyectoServiceImpl proyectoService)
        {
            _service = service;
            _versionService = versionService;
            _proyectoService = proyectoService;
        }

        private async Task<IActionResult?> ValidarPertenenciaProyectoAsync(int idProyecto)
        {
            var acceso = await _proyectoService.TieneAccesoAsync(idProyecto, User.GetIdUsuario(), User.GetRol());
            if (acceso is null) return NotFound($"El proyecto {idProyecto} no existe.");
            return acceso == false ? Forbid() : null;
        }

        private async Task<IActionResult?> ValidarPertenenciaPorVersionAsync(int idVersionDiseno)
        {
            var version = await _versionService.GetByIdAsync(idVersionDiseno);
            if (version is null) return NotFound("La versión de diseño no existe.");
            return await ValidarPertenenciaProyectoAsync(version.Idproyecto);
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
            var noPertenece = await ValidarPertenenciaPorVersionAsync(idVersionDiseno);
            if (noPertenece is not null) return noPertenece;

            return Ok(await _service.GetByVersionDisenoAsync(idVersionDiseno));
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Modeloimportado modelo)
        {
            var noPertenece = await ValidarPertenenciaPorVersionAsync(modelo.Idversiondiseno);
            if (noPertenece is not null) return noPertenece;

            var creado = await _service.CrearAsync(modelo);
            return CreatedAtAction(nameof(GetById), new { id = creado.Idmodeloimportado }, creado);
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPut("{id}/transform")]
        public async Task<IActionResult> ActualizarTransform(int id, [FromBody] Modeloimportado modelo)
        {
            if (id != modelo.Idmodeloimportado) return BadRequest("El id de la URL no coincide con el del body.");

            var noPertenece = await ValidarPertenenciaPorVersionAsync(modelo.Idversiondiseno);
            if (noPertenece is not null) return noPertenece;

            var actualizado = await _service.ActualizarTransformAsync(modelo);
            return actualizado ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var modelo = await _service.GetByIdAsync(id);
            if (modelo is null) return NotFound();

            var noPertenece = await ValidarPertenenciaPorVersionAsync(modelo.Idversiondiseno);
            if (noPertenece is not null) return noPertenece;

            var eliminado = await _service.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}