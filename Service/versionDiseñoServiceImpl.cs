using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public interface versionDiseñoServiceImpl
    {
        Task<IEnumerable<Versiondiseno>> GetAllAsync();
        Task<Versiondiseno?> GetByIdAsync(int id);
        Task<IEnumerable<Versiondiseno>> GetByProyectoAsync(int idProyecto);
        Task<Versiondiseno?> GetVersionActualAsync(int idProyecto);
        Task<Versiondiseno> CrearAsync(Versiondiseno version);
        Task<bool> MarcarComoActualAsync(int id, int idProyecto);
        Task<bool> EliminarAsync(int id);
    }
}
