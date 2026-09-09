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
    public class elementoeEstructuralController : ControllerBase
    {
        private readonly elementoEstructuralServiceImpl _service;
        private readonly versionDiseñoServiceImpl _versionService;
        private readonly proyectoServiceImpl _proyectoService;

        public elementoeEstructuralController(
            elementoEstructuralServiceImpl service,
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
            var elemento = await _service.GetByIdAsync(id);
            return elemento is null ? NotFound() : Ok(elemento);
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
        public async Task<IActionResult> Crear([FromBody] Elementoestructural elemento)
        {
            var noPertenece = await ValidarPertenenciaPorVersionAsync(elemento.Idversiondiseno);
            if (noPertenece is not null) return noPertenece;

            var creado = await _service.CrearAsync(elemento);
            return CreatedAtAction(nameof(GetById), new { id = creado.Idelementoestructural }, creado);
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Elementoestructural elemento)
        {
            if (id != elemento.Idelementoestructural) return BadRequest("El id de la URL no coincide con el del body.");

            var noPertenece = await ValidarPertenenciaPorVersionAsync(elemento.Idversiondiseno);
            if (noPertenece is not null) return noPertenece;

            var actualizado = await _service.ActualizarAsync(elemento);
            return actualizado ? NoContent() : NotFound();
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var elemento = await _service.GetByIdAsync(id);
            if (elemento is null) return NotFound();

            var noPertenece = await ValidarPertenenciaPorVersionAsync(elemento.Idversiondiseno);
            if (noPertenece is not null) return noPertenece;

            var eliminado = await _service.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}