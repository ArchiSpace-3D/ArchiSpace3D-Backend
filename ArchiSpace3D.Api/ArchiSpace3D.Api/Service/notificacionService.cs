using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Hubs;
using ArchiSpace3D.Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace ArchiSpace3D.Api.Service
{
    public class notificacionService : notificacionServiceImpl
    {
        private readonly notificacionDAOImpl _dao;
        private readonly proyectoDAOImpl _proyectoDao;
        private readonly usuarioDAOImpl _usuarioDao;
        private readonly IHubContext<SalaColaborativaHub> _hubContext;
        private readonly pushNotificationServiceImpl _pushService;

        public notificacionService(
            notificacionDAOImpl dao,
            proyectoDAOImpl proyectoDao,
            usuarioDAOImpl usuarioDao,
            IHubContext<SalaColaborativaHub> hubContext,
            pushNotificationServiceImpl pushService)
        {
            _dao = dao;
            _proyectoDao = proyectoDao;
            _usuarioDao = usuarioDao;
            _hubContext = hubContext;
            _pushService = pushService;
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
            await EmpujarPushAsync(creada);
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

            
            await CrearAsync(notificacion);
        }

        private async Task EmpujarEnVivoAsync(Notificacion notificacion)
        {
            var grupo = notificacion.Idproyecto.ToString();
            await _hubContext.Clients.Group(grupo).SendAsync("NuevaNotificacion", notificacion);
        }

        private async Task EmpujarPushAsync(Notificacion notificacion)
        {

            var proyecto = await _proyectoDao.GetByIdAsync(notificacion.Idproyecto);
            if (proyecto is null)
            {
                return;
            }


            var cliente = await _usuarioDao.GetByIdAsync(proyecto.Idcliente);
            if (cliente is null)
            {
                return;
            }


            if (string.IsNullOrEmpty(cliente.Fcmtoken))
            {
                return;
            }

            await _pushService.EnviarNotificacionAsync(
                cliente.Fcmtoken,
                titulo: proyecto.Nombre,
                cuerpo: notificacion.Mensaje,
                data: new Dictionary<string, string>
                {
            { "idProyecto", notificacion.Idproyecto.ToString() },
            { "tipo", notificacion.Tipo ?? "" }
                }
            );
        }
    }
}