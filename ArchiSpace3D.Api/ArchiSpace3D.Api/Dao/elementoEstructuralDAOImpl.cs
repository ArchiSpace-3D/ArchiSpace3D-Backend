using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Dao
{
    public interface elementoEstructuralDAOImpl
    {
        Task<IEnumerable<Elementoestructural>> GetAllAsync();
        Task<Elementoestructural?> GetByIdAsync(int id);
        Task<IEnumerable<Elementoestructural>> GetByVersionDiseñoAsync(int idVersionDiseno);
        Task<Elementoestructural> CreateAsync(Elementoestructural elemento);
        Task<bool> UpdateAsync(Elementoestructural elemento);
        Task<bool> DeleteAsync(int id);
    }
}
