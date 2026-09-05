using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public interface proyectoServiceImpl
    {
        Task<IEnumerable<Proyecto>> GetAllAsync();
        Task<Proyecto?> GetByIdAsync(int id);
        Task<IEnumerable<Proyecto>> GetByArquitectoAsync(int idArquitecto);
        Task<IEnumerable<Proyecto>> GetByClienteAsync(int idCliente);
        Task<Proyecto> CrearAsync(Proyecto proyecto);
        Task<bool> ActualizarAsync(Proyecto proyecto);
        Task<bool> EliminarAsync(int id);
        Task<bool?> TieneAccesoAsync(int idProyecto, int idUsuario, string rol);
    }
}