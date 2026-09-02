using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public interface usuarioDAOImpl
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByNumeroDocumentoAsync(string numeroDocumento);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<bool> UpdateAsync(Usuario usuario);
        Task<bool> DeleteAsync(int id);
    }
}
