using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    // Capa delgada por ahora: Elementoestructural no tiene reglas de negocio
    // adicionales todavía, solo delega al DAO. Igual se mantiene la capa para
    // no romper Controller -> Service -> Dao cuando aparezcan reglas nuevas.
    public class elementoEstructuralService : elementoEstructuralServiceImpl
    {
        private readonly elementoEstructuralDAOImpl _dao;

        public elementoEstructuralService(elementoEstructuralDAOImpl dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<Elementoestructural>> GetAllAsync() => await _dao.GetAllAsync();

        public async Task<Elementoestructural?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);

        public async Task<IEnumerable<Elementoestructural>> GetByVersionDisenoAsync(int idVersionDiseno) =>
            await _dao.GetByVersionDiseñoAsync(idVersionDiseno);

        public async Task<Elementoestructural> CrearAsync(Elementoestructural elemento) =>
            await _dao.CreateAsync(elemento);

        public async Task<bool> ActualizarAsync(Elementoestructural elemento) => await _dao.UpdateAsync(elemento);

        public async Task<bool> EliminarAsync(int id) => await _dao.DeleteAsync(id);
    }
}