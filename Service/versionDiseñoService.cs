using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public class versionDisenoService : versionDiseñoServiceImpl
    {
        private readonly versiondiseñoDAOImpl _dao;

        public versionDisenoService(versiondiseñoDAOImpl dao)
        {
            _dao = dao;
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

            return await _dao.CreateAsync(version);
        }

        public async Task<bool> MarcarComoActualAsync(int id, int idProyecto) =>
            await _dao.MarcarComoActualAsync(id, idProyecto);

        public async Task<bool> EliminarAsync(int id) => await _dao.DeleteAsync(id);
    }
}