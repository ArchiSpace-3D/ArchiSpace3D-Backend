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
    }
}
