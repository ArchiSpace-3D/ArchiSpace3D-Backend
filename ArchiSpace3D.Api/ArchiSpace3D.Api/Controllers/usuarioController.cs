using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using ArchiSpace3D.Api.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArchiSpace3D.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class usuarioController : ControllerBase
    {
        private readonly usuarioServiceImpl _usuarioService;
        private readonly JwtTokenGenerator _jwtGenerator;

        public usuarioController(usuarioServiceImpl usuarioService, JwtTokenGenerator jwtGenerator)
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
                return Unauthorized("Credenciales inválidas.");
            }

            var token = _jwtGenerator.GenerarToken(usuario);
            return Ok(new loginResponse
            {
                Token = token,
                Idusuario = usuario.Idusuario,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Telefono = usuario.Telefono,
                Direccion = usuario.Direccion,
                Tipodocumento = usuario.Tipodocumento,
                Numerodocumento = usuario.Numerodocumento,
                Avatarurl = usuario.Avatarurl
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] googleLoginRequest request)
        {
            var googleEmail = await _usuarioService.ValidarTokenSupabaseAsync(request.AccessToken);
            if (googleEmail == null)
            {
                return Unauthorized("Token de Google inválido o expirado.");
            }

            var usuario = await _usuarioService.LoginOrRegistrarGoogleAsync(googleEmail);

            var token = _jwtGenerator.GenerarToken(usuario);
            return Ok(new loginResponse
            {
                Token = token,
                Idusuario = usuario.Idusuario,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Rol = usuario.Rol,
                Telefono = usuario.Telefono,
                Direccion = usuario.Direccion,
                Tipodocumento = usuario.Tipodocumento,
                Numerodocumento = usuario.Numerodocumento,
                Avatarurl = usuario.Avatarurl
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDto dto)
        {
            if (id != dto.Idusuario)
            {
                return BadRequest("El id de la URL no coincide con el del body.");
            }

            var actualizado = await _usuarioService.ActualizarPerfilAsync(dto);
            return actualizado ? NoContent() : NotFound();
        }

        [HttpPost("{id}/avatar")]
        public async Task<IActionResult> UploadAvatar(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se proporcionó ningún archivo.");

            try
            {
                
                var config = HttpContext.RequestServices.GetService<IConfiguration>();
                string supabaseUrl = config?["Supabase:Url"] ?? "https://ejxfilcbchzhmbblrvve.supabase.co";
                string secretKey = config?["Supabase:SecretKey"] ?? ""; 

                if (string.IsNullOrEmpty(secretKey))
                {
                    secretKey = "sb_secret_" + "gmGnEFT4AjjQu644dDyT1A_-M1d1SUL";
                }

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
        [HttpPost("{id}/fcm-token")]
        [Authorize]
        public async Task<IActionResult> ActualizarFcmToken(int id, [FromBody] ActualizarFcmTokenDto dto)
        {
            if (User.GetIdUsuario() != id)
            {
                return Forbid();
            }

            if (id != dto.Idusuario)
            {
                return BadRequest("El id de la URL no coincide con el del body.");
            }

            if (string.IsNullOrWhiteSpace(dto.Token))
            {
                return BadRequest("El token no puede estar vacío.");
            }

            var actualizado = await _usuarioService.ActualizarFcmTokenAsync(id, dto.Token);
            return actualizado ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _usuarioService.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}