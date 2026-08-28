using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public class medicionService : medicionServiceImpl
    {
        private readonly medicionDAOImpl _dao;

        public medicionService(medicionDAOImpl dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<Medicion>> GetAllAsync() => await _dao.GetAllAsync();

        public async Task<Medicion?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);

        public async Task<IEnumerable<Medicion>> GetByProyectoAsync(int idProyecto) =>
            await _dao.GetByProyectoAsync(idProyecto);

        public async Task<Medicion> CreateAsync(Medicion medicion) => await _dao.CreateAsync(medicion);

        public async Task<bool> DeleteAsync(int id) => await _dao.DeleteAsync(id);
    }
}