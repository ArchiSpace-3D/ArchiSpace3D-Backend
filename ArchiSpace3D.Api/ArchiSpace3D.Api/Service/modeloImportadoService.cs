using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public class modeloImportadoService : modeloImportadoServiceImpl
    {
        private readonly modeloimportadoDAOImpl _dao;

        public modeloImportadoService(modeloimportadoDAOImpl dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<Modeloimportado>> GetAllAsync() => await _dao.GetAllAsync();

        public async Task<Modeloimportado?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);

        public async Task<IEnumerable<Modeloimportado>> GetByVersionDisenoAsync(int idVersionDiseno) =>
            await _dao.GetByVersionDisenoAsync(idVersionDiseno);

        public async Task<Modeloimportado> CrearAsync(Modeloimportado modelo) => await _dao.CreateAsync(modelo);

        public async Task<bool> ActualizarTransformAsync(Modeloimportado modelo) =>
            await _dao.UpdateTransformAsync(modelo);

        public async Task<bool> EliminarAsync(int id) => await _dao.DeleteAsync(id);
    }
}