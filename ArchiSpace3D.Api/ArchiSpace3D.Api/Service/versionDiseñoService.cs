using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public class versionDiseñoService : versionDiseñoServiceImpl
    {
        private readonly versiondiseñoDAOImpl _dao;
        private readonly notificacionServiceImpl _notificacionService;

        public versionDiseñoService(versiondiseñoDAOImpl dao, notificacionServiceImpl notificacionService)
        {
            _dao = dao;
            _notificacionService = notificacionService;
        }

        public async Task<IEnumerable<Versiondiseno>> GetAllAsync() => await _dao.GetAllAsync();

        public async Task<Versiondiseno?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);

        public async Task<IEnumerable<Versiondiseno>> GetByProyectoAsync(int idProyecto) =>
            await _dao.GetByProyectoAsync(idProyecto);

        public async Task<Versiondiseno?> GetVersionActualAsync(int idProyecto) =>
            await _dao.GetVersionActualAsync(idProyecto);

        public async Task<Versiondiseno> CrearAsync(Versiondiseno version)
        {
            if (await _dao.ExistsNumeroVersionAsync(version.Idproyecto, version.Numeroversion))
            {
                throw new InvalidOperationException("Ya existe esa versión de diseño para este proyecto.");
            }

            var creada = await _dao.CreateAsync(version);

            await _notificacionService.NotificarCambioAsync(creada.Idproyecto, "version_creada",
                $"El arquitecto creó la versión {creada.Numeroversion} del diseño.", creada.Idversiondiseno);

            return creada;
        }

        public async Task<bool> MarcarComoActualAsync(int id, int idProyecto)
        {
            var actualizado = await _dao.MarcarComoActualAsync(id, idProyecto);

            if (actualizado)
            {
                await _notificacionService.NotificarCambioAsync(idProyecto, "version_actual_cambiada",
                    "El arquitecto cambió la versión actual del diseño.", id);
            }

            return actualizado;
        }

        public async Task<bool> EliminarAsync(int id) => await _dao.DeleteAsync(id);
    }
}