using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public class espacioFisicoService : espacioFisicoServiceImpl
    {
        private readonly espacioFisicoDAOImpl _dao;

        public espacioFisicoService(espacioFisicoDAOImpl dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<Espaciofisico>> GetAllAsync() => await _dao.GetAllAsync();

        public async Task<Espaciofisico?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);

        public async Task<Espaciofisico?> GetByProyectoAsync(int idProyecto) =>
            await _dao.GetByProyectoAsync(idProyecto);

        public async Task<Espaciofisico> CrearAsync(Espaciofisico espacio)
        {
            // Regla de negocio: un proyecto solo puede tener UN espacio físico (relación uno-a-uno)
            if (await _dao.ExistsByProyectoAsync(espacio.Idproyecto))
            {
                throw new InvalidOperationException("Este proyecto ya tiene un espacio físico registrado.");
            }

            return await _dao.CreateAsync(espacio);
        }

        public async Task<bool> ActualizarAsync(Espaciofisico espacio) => await _dao.UpdateAsync(espacio);

        public async Task<bool> EliminarAsync(int id) => await _dao.DeleteAsync(id);
    }
}