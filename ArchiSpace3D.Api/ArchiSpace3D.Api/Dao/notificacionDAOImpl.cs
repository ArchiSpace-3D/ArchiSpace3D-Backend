using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Dao
{
    public interface notificacionDAOImpl
    {
        Task<IEnumerable<Notificacion>> GetAllAsync();
        Task<Notificacion?> GetByIdAsync(int id);
        Task<IEnumerable<Notificacion>> GetByProyectoAsync(int idProyecto);
        Task<IEnumerable<Notificacion>> GetNoLeidasByProyectoAsync(int idProyecto);
        Task<Notificacion> CreateAsync(Notificacion notificacion);
        Task<bool> MarcarComoLeidaAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
