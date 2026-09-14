using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class usuarioController : ControllerBase
    {
        private readonly usuarioServiceImpl _usuarioService;
        private readonly ArchiSpace3D.Api.Util.JwtTokenGenerator _jwtGenerator;

        public usuarioController(usuarioServiceImpl usuarioService, ArchiSpace3D.Api.Util.JwtTokenGenerator jwtGenerator)
        {
            _usuarioService = usuarioService;
            _jwtGenerator = jwtGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _usuarioService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            return usuario is null ? NotFound() : Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
        {
            try
            {
                var creado = await _usuarioService.RegistrarAsync(usuario);
                return CreatedAtAction(nameof(GetById), new { id = creado.Idusuario }, creado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _usuarioService.LoginAsync(request.Email, request.Contrasena);
            if (usuario == null)
            {
                return Unauthorized("Credenciales invÃ¡lidas.");
            }

            string token = _jwtGenerator.GenerarToken(usuario);

            return Ok(new { token = token, idusuario = usuario.Idusuario, nombre = usuario.Nombre, apellido = usuario.Apellido, email = usuario.Email, telefono = usuario.Telefono, direccion = usuario.Direccion, tipodocumento = usuario.Tipodocumento, numerodocumento = usuario.Numerodocumento, avatarurl = usuario.Avatarurl, rol = usuario.Rol });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDto dto)
        {
            if (id != dto.Idusuario)
            {
                return BadRequest("El id de la URL no coincide con el del body.");
            }

            var usuario = new Usuario
            {
                Idusuario = dto.Idusuario,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                Tipodocumento = dto.Tipodocumento,
                Numerodocumento = dto.Numerodocumento,
                Avatarurl = dto.Avatarurl
            };

            var actualizado = await _usuarioService.ActualizarAsync(usuario);
            return actualizado ? NoContent() : NotFound();
        }

        [HttpPost("{id}/avatar")]
        public async Task<IActionResult> UploadAvatar(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se proporcionó ningún archivo.");

            try
            {
                // Leer configuración desde appsettings.json o variables de entorno (Railway)
                var config = HttpContext.RequestServices.GetService<IConfiguration>();
                string supabaseUrl = config?["Supabase:Url"] ?? "https://ejxfilcbchzhmbblrvve.supabase.co";
                string secretKey = config?["Supabase:SecretKey"] ?? ""; 

                if (string.IsNullOrEmpty(secretKey))
                    return StatusCode(500, "Error de configuración: Supabase Secret Key no encontrada.");

                string fileName = $"avatar_{id}_{DateTime.UtcNow.Ticks}.jpg";
                string storageUrl = $"{supabaseUrl}/storage/v1/object/avatars/{fileName}";

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("apikey", secretKey);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");

                using var stream = file.OpenReadStream();
                var content = new StreamContent(stream);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "image/jpeg");

                var response = await httpClient.PostAsync(storageUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, $"Error subiendo a Supabase: {error}");
                }

                string publicUrl = $"{supabaseUrl}/storage/v1/object/public/avatars/{fileName}";

                var user = await _usuarioService.GetByIdAsync(id);
                if (user != null)
                {
                    user.Avatarurl = publicUrl;
                    await _usuarioService.ActualizarAsync(user);
                }

                return Ok(new { url = publicUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _usuarioService.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}


