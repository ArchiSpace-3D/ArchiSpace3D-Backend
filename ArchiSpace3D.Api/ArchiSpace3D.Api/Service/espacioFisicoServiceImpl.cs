using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public interface espacioFisicoServiceImpl
    {
        Task<IEnumerable<Espaciofisico>> GetAllAsync();
        Task<Espaciofisico?> GetByIdAsync(int id);
        Task<Espaciofisico?> GetByProyectoAsync(int idProyecto);
        Task<Espaciofisico> CrearAsync(Espaciofisico espacio);
        Task<bool> ActualizarAsync(Espaciofisico espacio);
        Task<bool> EliminarAsync(int id);
    }
}
