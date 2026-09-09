using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public class notificacionService : notificacionServiceImpl
    {
        private readonly notificacionDAOImpl _dao;

        public notificacionService(notificacionDAOImpl dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<Notificacion>> GetAllAsync() => await _dao.GetAllAsync();

        public async Task<Notificacion?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);

        public async Task<IEnumerable<Notificacion>> GetByProyectoAsync(int idProyecto) =>
            await _dao.GetByProyectoAsync(idProyecto);

        public async Task<IEnumerable<Notificacion>> GetNoLeidasByProyectoAsync(int idProyecto) =>
            await _dao.GetNoLeidasByProyectoAsync(idProyecto);

        public async Task<Notificacion> CrearAsync(Notificacion notificacion) => await _dao.CreateAsync(notificacion);

        public async Task<bool> MarcarComoLeidaAsync(int id) => await _dao.MarcarComoLeidaAsync(id);

        public async Task<bool> EliminarAsync(int id) => await _dao.DeleteAsync(id);
    }
}