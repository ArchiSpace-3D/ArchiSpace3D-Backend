using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public class invitacionService : invitacionServiceImpl
    {
        private readonly invitacionDAOImpl _dao;

        public invitacionService(invitacionDAOImpl dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<Invitacion>> GetAllAsync() => await _dao.GetAllAsync();

        public async Task<Invitacion?> GetByIdAsync(int id) => await _dao.GetByIdAsync(id);

        public async Task<Invitacion?> GetByCodigoAsync(string codigo) => await _dao.GetByCodigoAsync(codigo);

        public async Task<IEnumerable<Invitacion>> GetByProyectoAsync(int idProyecto) =>
            await _dao.GetByProyectoAsync(idProyecto);

        public async Task<IEnumerable<Invitacion>> GetByArquitectoAsync(int idArquitecto) =>
            await _dao.GetByArquitectoAsync(idArquitecto);

        public async Task<Invitacion> CrearAsync(Invitacion invitacion)
        {
            if (await _dao.ExistsByCodigoAsync(invitacion.Codigo))
            {
                throw new InvalidOperationException("Ya existe una invitación con ese código.");
            }

            return await _dao.CreateAsync(invitacion);
        }

        // Regla de negocio nueva (no estaba en el DAO): antes de marcar la
        // invitación como usada, se valida que exista, que no esté ya usada,
        // y que no haya expirado. Esto es exactamente el tipo de lógica que
        // pertenece al Service, no al DAO ni al Controller.
        public async Task<bool> UsarInvitacionAsync(string codigo, int idClienteUsado)
        {
            var invitacion = await _dao.GetByCodigoAsync(codigo);

            if (invitacion is null)
            {
                throw new InvalidOperationException("El código de invitación no existe.");
            }

            if (invitacion.Usada == true)
            {
                throw new InvalidOperationException("Esta invitación ya fue usada.");
            }

            if (invitacion.Fechaexpiracion.HasValue && invitacion.Fechaexpiracion.Value < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Esta invitación ya expiró.");
            }

            return await _dao.MarcarComoUsadaAsync(invitacion.Idinvitacion, idClienteUsado);
        }

        public async Task<bool> EliminarAsync(int id) => await _dao.DeleteAsync(id);
    }
}