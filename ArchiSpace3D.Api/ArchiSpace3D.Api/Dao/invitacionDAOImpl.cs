using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Dao
{
    public interface invitacionDAOImpl
    {
        Task<IEnumerable<Invitacion>> GetAllAsync();
        Task<Invitacion?> GetByIdAsync(int id);
        Task<Invitacion?> GetByCodigoAsync(string codigo);
        Task<IEnumerable<Invitacion>> GetByProyectoAsync(int idProyecto);
        Task<IEnumerable<Invitacion>> GetByArquitectoAsync(int idArquitecto);
        Task<bool> ExistsByCodigoAsync(string codigo);
        Task<Invitacion> CreateAsync(Invitacion invitacion);
        Task<bool> MarcarComoUsadaAsync(int id, int idClienteUsado);
        Task<bool> DeleteAsync(int id);
    }
}
