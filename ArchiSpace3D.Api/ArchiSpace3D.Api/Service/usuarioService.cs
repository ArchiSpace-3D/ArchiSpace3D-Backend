using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;
using System.Text.Json;

namespace ArchiSpace3D.Api.Service
{
    public class usuarioService : usuarioServiceImpl
    {
        private readonly usuarioDAOImpl _usuarioDao;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public usuarioService(usuarioDAOImpl usuarioDao, HttpClient httpClient, IConfiguration configuration)
        {
            _usuarioDao = usuarioDao;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _usuarioDao.GetAllAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _usuarioDao.GetByIdAsync(id);
        }

        public async Task<Usuario> RegistrarAsync(Usuario usuario)
        {
            usuario.Email = usuario.Email.Trim();
            usuario.Numerodocumento = string.IsNullOrWhiteSpace(usuario.Numerodocumento)
                ? null
                : usuario.Numerodocumento.Trim();

            if (string.IsNullOrWhiteSpace(usuario.Rol) ||
                (!string.Equals(usuario.Rol, "Arquitecto", StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(usuario.Rol, "Cliente", StringComparison.OrdinalIgnoreCase)))
            {
                usuario.Rol = "Cliente";
            }
            else
            {
                usuario.Rol = string.Equals(usuario.Rol, "Arquitecto", StringComparison.OrdinalIgnoreCase)
                    ? "Arquitecto"
                    : "Cliente";
            }

            if (await _usuarioDao.ExistsByEmailAsync(usuario.Email))
            {
                throw new InvalidOperationException("Ya existe un usuario con ese email.");
            }

            if (!string.IsNullOrEmpty(usuario.Numerodocumento) &&
                await _usuarioDao.ExistsByNumeroDocumentoAsync(usuario.Numerodocumento))
            {
                throw new InvalidOperationException("Ya existe un usuario con ese número de documento.");
            }

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            usuario.Activo ??= true;

            return await _usuarioDao.CreateAsync(usuario);
        }

        public async Task<Usuario?> LoginAsync(string email, string password)
        {
            var usuario = await _usuarioDao.GetByEmailAsync(email.Trim());
            if (usuario == null || usuario.Activo == false)
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, usuario.Contrasena))
            {
                return null;
            }

            return usuario;
        }

        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            return await _usuarioDao.UpdateAsync(usuario);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            return await _usuarioDao.DeleteAsync(id);
        }

        public async Task<string?> ValidarTokenSupabaseAsync(string accessToken)
        {
            var supabaseUrl = _configuration["Supabase:Url"];
            var supabaseAnonKey = _configuration["Supabase:AnonKey"];

            var request = new HttpRequestMessage(HttpMethod.Get, $"{supabaseUrl}/auth/v1/user");
            request.Headers.Add("Authorization", $"Bearer {accessToken}");
            request.Headers.Add("apikey", supabaseAnonKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : null;
        }

        public async Task<Usuario> LoginOrRegistrarGoogleAsync(string email)
        {
            var existente = await _usuarioDao.GetByEmailAsync(email.Trim());
            if (existente != null)
            {
                if (existente.Activo == false)
                    throw new InvalidOperationException("Esta cuenta se encuentra desactivada.");
                return existente;
            }

            var nuevo = new Usuario
            {
                Nombre = email.Split('@')[0],
                Apellido = "",
                Email = email.Trim(),
                Contrasena = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
                Rol = "Cliente",
                Activo = true,
                Fecharegistro = DateTime.UtcNow
            };

            return await _usuarioDao.CreateAsync(nuevo);
        }

        public async Task<bool> ActualizarPerfilAsync(ActualizarUsuarioDto dto)
        {
            var existente = await _usuarioDao.GetByIdAsync(dto.Idusuario);
            if (existente is null) return false;

            var nuevoDoc = string.IsNullOrWhiteSpace(dto.Numerodocumento)
                ? null
                : dto.Numerodocumento.Trim();

            if (!string.IsNullOrEmpty(nuevoDoc) &&
                !string.Equals(existente.Numerodocumento, nuevoDoc, StringComparison.OrdinalIgnoreCase))
            {
                if (await _usuarioDao.ExistsByNumeroDocumentoAsync(nuevoDoc))
                {
                    throw new InvalidOperationException("Ya existe otro usuario con ese número de documento.");
                }
            }

            existente.Nombre = dto.Nombre.Trim();
            existente.Apellido = dto.Apellido?.Trim() ?? "";
            existente.Telefono = dto.Telefono;
            existente.Direccion = dto.Direccion;
            if (dto.Tipodocumento != null)
                existente.Tipodocumento = string.IsNullOrWhiteSpace(dto.Tipodocumento) ? null : dto.Tipodocumento.Trim();
            if (dto.Numerodocumento != null)
                existente.Numerodocumento = nuevoDoc;

            if (!string.IsNullOrEmpty(dto.Avatarurl))
            {
                existente.Avatarurl = dto.Avatarurl;
            }

            if (dto.Activo.HasValue)
            {
                existente.Activo = dto.Activo.Value;
            }

            return await _usuarioDao.UpdateAsync(existente);
        }

        public async Task<bool> ActualizarFcmTokenAsync(int idUsuario, string token)
        {
            return await _usuarioDao.ActualizarFcmTokenAsync(idUsuario, token);
        }
    }
}
