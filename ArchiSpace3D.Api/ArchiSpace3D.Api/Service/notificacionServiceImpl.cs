using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public interface notificacionServiceImpl
    {
        Task<IEnumerable<Notificacion>> GetAllAsync();
        Task<Notificacion?> GetByIdAsync(int id);
        Task<IEnumerable<Notificacion>> GetByProyectoAsync(int idProyecto);
        Task<IEnumerable<Notificacion>> GetNoLeidasByProyectoAsync(int idProyecto);
        Task<Notificacion> CrearAsync(Notificacion notificacion);
        Task<bool> MarcarComoLeidaAsync(int id);
        Task<bool> EliminarAsync(int id);
        Task NotificarCambioAsync(int idProyecto, string tipo, string mensaje, int? idVersionDiseno = null);
    }
}