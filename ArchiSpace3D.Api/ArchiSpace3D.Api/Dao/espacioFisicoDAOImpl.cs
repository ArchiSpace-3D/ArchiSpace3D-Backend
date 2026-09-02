using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Dao
{
    public interface espacioFisicoDAOImpl
    {
        Task<IEnumerable<Espaciofisico>> GetAllAsync();
        Task<Espaciofisico?> GetByIdAsync(int id);
        Task<Espaciofisico?> GetByProyectoAsync(int idProyecto);
        Task<bool> ExistsByProyectoAsync(int idProyecto);
        Task<Espaciofisico> CreateAsync(Espaciofisico espacio);
        Task<bool> UpdateAsync(Espaciofisico espacio);
        Task<bool> DeleteAsync(int id);
    }
}
