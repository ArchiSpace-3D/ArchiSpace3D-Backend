using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public interface elementoEstructuralServiceImpl
    {
        Task<IEnumerable<Elementoestructural>> GetAllAsync();
        Task<Elementoestructural?> GetByIdAsync(int id);
        Task<IEnumerable<Elementoestructural>> GetByVersionDisenoAsync(int idVersionDiseno);
        Task<Elementoestructural> CrearAsync(Elementoestructural elemento);
        Task<bool> ActualizarAsync(Elementoestructural elemento);
        Task<bool> EliminarAsync(int id);
    }
}
