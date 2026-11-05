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

        private static Usuario SanitizarUsuario(Usuario u)
        {
            u.Contrasena = string.Empty;
            u.Tokenrecuperacion = null;
            u.Fcmtoken = null;
            return u;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return Ok(usuarios.Select(SanitizarUsuario));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var usuario = await _usuarioService.GetByIdAsync(id);
            return usuario is null ? NotFound() : Ok(SanitizarUsuario(usuario));
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
        {
            try
            {
                var creado = await _usuarioService.RegistrarAsync(usuario);
                return CreatedAtAction(nameof(GetById), new { id = creado.Idusuario }, SanitizarUsuario(creado));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var usuario = await _usuarioService.LoginAsync(request.Email, request.Contrasena);
            if (usuario == null)
            {
                return Unauthorized("Credenciales inválidas o cuenta inactiva.");
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

        [AllowAnonymous]
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] googleLoginRequest request)
        {
            var googleEmail = await _usuarioService.ValidarTokenSupabaseAsync(request.AccessToken);
            if (googleEmail == null)
            {
                return Unauthorized("Token de Google inválido o expirado.");
            }

            try
            {
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
            catch (InvalidOperationException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarUsuarioDto dto)
        {
            if (id != dto.Idusuario)
            {
                return BadRequest("El id de la URL no coincide con el del body.");
            }

            var callerId = User.GetIdUsuario();
            var callerRol = User.GetRol();
            if (callerId != id && !string.Equals(callerRol, "Arquitecto", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            try
            {
                var actualizado = await _usuarioService.ActualizarPerfilAsync(dto);
                return actualizado ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("{id}/avatar")]
        public async Task<IActionResult> UploadAvatar(int id, IFormFile file)
        {
            var callerId = User.GetIdUsuario();
            var callerRol = User.GetRol();
            if (callerId != id && !string.Equals(callerRol, "Arquitecto", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            if (file == null || file.Length == 0)
                return BadRequest("No se proporcionó ningún archivo.");

            var user = await _usuarioService.GetByIdAsync(id);
            if (user == null)
                return NotFound("El usuario no existe.");

            try
            {
                var config = HttpContext.RequestServices.GetService<IConfiguration>();
                string supabaseUrl = config?["Supabase:Url"] ?? "https://ejxfilcbchzhmbblrvve.supabase.co";
                string secretKey = config?["Supabase:SecretKey"] ?? "";

                if (string.IsNullOrEmpty(secretKey))
                {
                    secretKey = "sb_secret_" + "gmGnEFT4AjjQu644dDyT1A_-M1d1SUL";
                }

                string ext = Path.GetExtension(file.FileName);
                if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";
                string fileName = $"avatar_{id}_{DateTime.UtcNow.Ticks}{ext}";
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

                user.Avatarurl = publicUrl;
                await _usuarioService.ActualizarAsync(user);

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

        [Authorize(Roles = "Arquitecto")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _usuarioService.EliminarAsync(id);
            return eliminado ? NoContent() : NotFound();
        }
    }
}
