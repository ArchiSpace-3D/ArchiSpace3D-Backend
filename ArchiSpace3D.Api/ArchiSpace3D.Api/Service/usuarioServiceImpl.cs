using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public interface usuarioServiceImpl
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario> RegistrarAsync(Usuario usuario);
        Task<Usuario?> LoginAsync(string email, string password);
        Task<bool> ActualizarAsync(Usuario usuario);
        Task<bool> EliminarAsync(int id);
        Task<bool> ActualizarPerfilAsync(ActualizarUsuarioDto dto);
        Task<string?> ValidarTokenSupabaseAsync(string accessToken);
        Task<Usuario> LoginOrRegistrarGoogleAsync(string email);
        Task<bool> ActualizarFcmTokenAsync(int idUsuario, string token);
    }
}