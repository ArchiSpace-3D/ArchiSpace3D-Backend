using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public class proyectoService : proyectoServiceImpl
    {
        private readonly proyectoDAOImpl _proyectoDao;
        private readonly notificacionServiceImpl _notificacionService;

        public proyectoService(proyectoDAOImpl proyectoDao, notificacionServiceImpl notificacionService)
        {
            _proyectoDao = proyectoDao;
            _notificacionService = notificacionService;
        }

        public async Task<IEnumerable<Proyecto>> GetAllAsync() => await _proyectoDao.GetAllAsync();

        public async Task<Proyecto?> GetByIdAsync(int id) => await _proyectoDao.GetByIdAsync(id);

        public async Task<IEnumerable<Proyecto>> GetByArquitectoAsync(int idArquitecto) =>
            await _proyectoDao.GetByArquitectoAsync(idArquitecto);

        public async Task<IEnumerable<Proyecto>> GetByClienteAsync(int idCliente) =>
            await _proyectoDao.GetByClienteAsync(idCliente);

        public async Task<Proyecto> CrearAsync(Proyecto proyecto)
        {
            if (!string.IsNullOrEmpty(proyecto.Codigosalaactiva) &&
                await _proyectoDao.ExistByCodigoSalaActivaAsync(proyecto.Codigosalaactiva))
            {
                throw new InvalidOperationException("Ya existe un proyecto con ese código de sala activa.");
            }

            return await _proyectoDao.CreateAsync(proyecto);
        }

        public async Task<bool> ActualizarAsync(Proyecto proyecto)
        {
            var result = await _proyectoDao.UpdateAsync(proyecto);
            if (result)
            {
                await _notificacionService.NotificarCambioAsync(
                    proyecto.Idproyecto, 
                    "Edicion", 
                    $"El proyecto ha sido modificado.");
            }
            return result;
        }

        public async Task<bool> EliminarAsync(int id) => await _proyectoDao.DeleteAsync(id);

        

        public async Task<bool?> TieneAccesoAsync(int idProyecto, int idUsuario, string rol)
        {
            var proyecto = await _proyectoDao.GetByIdAsync(idProyecto);

            if (proyecto is null)
            {
                return null;
            }

            return rol == "Arquitecto"
                ? proyecto.Idarquitecto == idUsuario
                : proyecto.Idcliente == idUsuario;
        }
    }
}