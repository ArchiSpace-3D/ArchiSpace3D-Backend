using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Dao
{
    public interface modeloimportadoDAOImpl
    {
        Task<IEnumerable<Modeloimportado>> GetAllAsync();
        Task<Modeloimportado?> GetByIdAsync(int id);
        Task<IEnumerable<Modeloimportado>> GetByVersionDisenoAsync(int idVersionDiseno);
        Task<Modeloimportado> CreateAsync(Modeloimportado modelo);
        Task<bool> UpdateTransformAsync(Modeloimportado modelo);
        Task<bool> DeleteAsync(int id);
    }
}
