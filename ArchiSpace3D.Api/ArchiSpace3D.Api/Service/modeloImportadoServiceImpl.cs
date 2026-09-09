using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public interface modeloImportadoServiceImpl
    {
        Task<IEnumerable<Modeloimportado>> GetAllAsync();
        Task<Modeloimportado?> GetByIdAsync(int id);
        Task<IEnumerable<Modeloimportado>> GetByVersionDisenoAsync(int idVersionDiseno);
        Task<Modeloimportado> CrearAsync(Modeloimportado modelo);
        Task<bool> ActualizarTransformAsync(Modeloimportado modelo);
        Task<bool> EliminarAsync(int id);
    }
}
