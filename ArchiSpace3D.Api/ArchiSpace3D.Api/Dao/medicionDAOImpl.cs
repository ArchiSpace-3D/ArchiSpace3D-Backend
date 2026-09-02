using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Dao
{
    public interface medicionDAOImpl
    {
        Task<IEnumerable<Medicion>> GetAllAsync();
        Task<Medicion?> GetByIdAsync(int id);
        Task<IEnumerable<Medicion>> GetByProyectoAsync(int idProyecto);
        Task<Medicion> CreateAsync(Medicion medicion);
        Task<bool> DeleteAsync(int id);
    }
}
