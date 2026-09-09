using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public interface invitacionServiceImpl
    {
        Task<IEnumerable<Invitacion>> GetAllAsync();
        Task<Invitacion?> GetByIdAsync(int id);
        Task<Invitacion?> GetByCodigoAsync(string codigo);
        Task<IEnumerable<Invitacion>> GetByProyectoAsync(int idProyecto);
        Task<IEnumerable<Invitacion>> GetByArquitectoAsync(int idArquitecto);
        Task<Invitacion> CrearAsync(Invitacion invitacion);
        Task<bool> UsarInvitacionAsync(string codigo, int idClienteUsado);
        Task<bool> EliminarAsync(int id);
    }
}
