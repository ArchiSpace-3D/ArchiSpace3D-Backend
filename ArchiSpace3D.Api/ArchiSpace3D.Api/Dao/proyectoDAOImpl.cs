using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Dao
{
    public interface proyectoDAOImpl
    {
        Task<IEnumerable<Proyecto>> GetAllAsync();
        Task<Proyecto?> GetByIdAsync(int id);
        Task<IEnumerable<Proyecto>> GetByArquitectoAsync(int idArquitecto);
        Task<IEnumerable<Proyecto>> GetByClienteAsync(int idCliente);
        Task<bool> ExistByCodigoSalaActivaAsync(string codigoSala);
        Task<Proyecto> CreateAsync(Proyecto proyecto);
        Task<bool> UpdateAsync(Proyecto proyecto);
        Task<bool> DeleteAsync(int id);
    }
}
