using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Dao
{
    public interface versiondiseñoDAOImpl
    {
        Task<IEnumerable<Versiondiseno>> GetAllAsync();
        Task<Versiondiseno?> GetByIdAsync(int id);
        Task<IEnumerable<Versiondiseno>> GetByProyectoAsync(int idProyecto);
        Task<Versiondiseno?> GetVersionActualAsync(int idProyecto);
        Task<bool> ExistsNumeroVersionAsync(int idProyecto, int numeroVersion);
        Task<Versiondiseno> CreateAsync(Versiondiseno version);
        Task<bool> MarcarComoActualAsync(int id, int idProyecto);
        Task<bool> DeleteAsync(int id);
    }
}
