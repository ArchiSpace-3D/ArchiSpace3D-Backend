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
    public class MedicionController : ControllerBase
    {
        private readonly medicionServiceImpl _service;
        private readonly proyectoServiceImpl _proyectoService;

        public MedicionController(medicionServiceImpl service, proyectoServiceImpl proyectoService)
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
            var medicion = await _service.GetByIdAsync(id);
            return medicion is null ? NotFound() : Ok(medicion);
        }

        [HttpGet("proyecto/{idProyecto}")]
        public async Task<IActionResult> GetByProyecto(int idProyecto)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(idProyecto);
            if (noPertenece is not null) return noPertenece;

            return Ok(await _service.GetByProyectoAsync(idProyecto));
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Medicion medicion)
        {
            var noPertenece = await ValidarPertenenciaProyectoAsync(medicion.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            var creada = await _service.CreateAsync(medicion);
            return CreatedAtAction(nameof(GetById), new { id = creada.Idmedicion }, creada);
        }

        [Authorize(Roles = "Arquitecto")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var medicion = await _service.GetByIdAsync(id);
            if (medicion is null) return NotFound();

            var noPertenece = await ValidarPertenenciaProyectoAsync(medicion.Idproyecto);
            if (noPertenece is not null) return noPertenece;

            var eliminado = await _service.DeleteAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}