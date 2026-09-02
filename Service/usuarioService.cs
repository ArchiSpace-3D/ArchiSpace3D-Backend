using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public class usuarioService : usuarioServiceImpl
    {
        private readonly usuarioDAOImpl _usuarioDao;

        public usuarioService(usuarioDAOImpl usuarioDao)
        {
            _usuarioDao = usuarioDao;
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
            // Regla de negocio: email único
            if (await _usuarioDao.ExistsByEmailAsync(usuario.Email))
            {
                throw new InvalidOperationException("Ya existe un usuario con ese email.");
            }

            // Regla de negocio: número de documento único (si lo mandaron)
            if (!string.IsNullOrEmpty(usuario.Numerodocumento) &&
                await _usuarioDao.ExistsByNumeroDocumentoAsync(usuario.Numerodocumento))
            {
                throw new InvalidOperationException("Ya existe un usuario con ese número de documento.");
            }

            // Regla de negocio: la contraseña nunca se guarda en texto plano
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

            return await _usuarioDao.CreateAsync(usuario);
        }

        public async Task<Usuario?> LoginAsync(string email, string password)
        {
            var usuario = await _usuarioDao.GetByEmailAsync(email);
            if (usuario == null)
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
    }
}