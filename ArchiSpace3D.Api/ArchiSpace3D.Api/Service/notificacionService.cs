using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Hubs;
using ArchiSpace3D.Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace ArchiSpace3D.Api.Service
{
    public class notificacionService : notificacionServiceImpl
    {
        private readonly notificacionDAOImpl _dao;
        private readonly IHubContext<SalaColaborativaHub> _hubContext;

        public notificacionService(notificacionDAOImpl dao, IHubContext<SalaColaborativaHub> hubContext)
        {
            _dao = dao;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<Notificacion>> GetAllAsync() => await _dao.GetAllAsync();

        public async Task<Notificacion?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);

        public async Task<IEnumerable<Notificacion>> GetByProyectoAsync(int idProyecto) =>
            await _dao.GetByProyectoAsync(idProyecto);

        public async Task<IEnumerable<Notificacion>> GetNoLeidasByProyectoAsync(int idProyecto) =>
            await _dao.GetNoLeidasByProyectoAsync(idProyecto);

        public async Task<Notificacion> CrearAsync(Notificacion notificacion)
        {
            var creada = await _dao.CreateAsync(notificacion);
            await EmpujarEnVivoAsync(creada);
            return creada;
        }

        public async Task<bool> MarcarComoLeidaAsync(int id) => await _dao.MarcarComoLeidaAsync(id);

        public async Task<bool> EliminarAsync(int id) => await _dao.DeleteAsync(id);

        public async Task NotificarCambioAsync(int idProyecto, string tipo, string mensaje, int? idVersionDiseno = null)
        {
            var notificacion = new Notificacion
            {
                Idproyecto = idProyecto,
                Idversiondiseno = idVersionDiseno,
                Tipo = tipo,
                Mensaje = mensaje
            };

            // Reutiliza CrearAsync -> mismo camino de guardado + push que una
            // notificación creada manualmente desde el Controller.
            await CrearAsync(notificacion);
        }

        private async Task EmpujarEnVivoAsync(Notificacion notificacion)
        {
            var grupo = notificacion.Idproyecto.ToString();
            await _hubContext.Clients.Group(grupo).SendAsync("NuevaNotificacion", notificacion);
        }
    }
}