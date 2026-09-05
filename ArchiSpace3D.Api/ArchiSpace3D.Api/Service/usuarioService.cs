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
            var usuarios = await _usuarioDao.GetAllAsync();
            foreach (var usuario in usuarios)
            {
                LimpiarDatosSensibles(usuario);
            }
            return usuarios;
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            var usuario = await _usuarioDao.GetByIdAsync(id);
            return usuario is null ? null : LimpiarDatosSensibles(usuario);
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

            var creado = await _usuarioDao.CreateAsync(usuario);
            return LimpiarDatosSensibles(creado);
        }

        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            return await _usuarioDao.UpdateAsync(usuario);
        }

        public async Task<bool> EliminarAsync(int id)
        {
            return await _usuarioDao.DeleteAsync(id);
        }

        // NUEVO: se llama antes de devolver CUALQUIER Usuario hacia afuera del
        // Service. Reutiliza el mismo modelo Usuario (nada de DTO nuevo) --
        // solo vacía los campos que nunca deben viajar en una respuesta HTTP:
        // el hash de la contraseña y los datos de recuperación de cuenta.
        private static Usuario LimpiarDatosSensibles(Usuario usuario)
        {
            usuario.Contrasena = string.Empty;
            usuario.Tokenrecuperacion = null;
            usuario.Expiraciontoken = null;
            return usuario;
        }
    }
}