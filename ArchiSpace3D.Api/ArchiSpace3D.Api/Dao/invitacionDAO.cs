using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Dao
{
    public class invitacionDAO : invitacionDAOImpl
    {
        private readonly ArchiSpaceContext _context;

        public invitacionDAO(ArchiSpaceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invitacion>> GetAllAsync()
        {
            return await _context.Invitacions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Invitacion?> GetByIdAsync(int id)
        {
            return await _context.Invitacions
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Idinvitacion == id);
        }

        public async Task<Invitacion?> GetByCodigoAsync(string codigo)
        {
            
            return await _context.Invitacions
                .FirstOrDefaultAsync(i => i.Codigo == codigo);
        }

        public async Task<IEnumerable<Invitacion>> GetByProyectoAsync(int idProyecto)
        {
            return await _context.Invitacions
                .AsNoTracking()
                .Where(i => i.Idproyecto == idProyecto)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invitacion>> GetByArquitectoAsync(int idArquitecto)
        {
            return await _context.Invitacions
                .AsNoTracking()
                .Where(i => i.Idarquitecto == idArquitecto)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCodigoAsync(string codigo)
        {
            return await _context.Invitacions
                .AsNoTracking()
                .AnyAsync(i => i.Codigo == codigo);
        }

        public async Task<Invitacion> CreateAsync(Invitacion invitacion)
        {
            invitacion.Fechacreacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            invitacion.Usada ??= false;

            _context.Invitacions.Add(invitacion);
            await _context.SaveChangesAsync();
            return invitacion;
        }

        public async Task<bool> MarcarComoUsadaAsync(int id, int idClienteUsado)
        {
            var existente = await _context.Invitacions
                .FirstOrDefaultAsync(i => i.Idinvitacion == id);

            if (existente is null)
            {
                return false;
            }

            existente.Usada = true;
            existente.Idclienteusado = idClienteUsado;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await _context.Invitacions
                .FirstOrDefaultAsync(i => i.Idinvitacion == id);

            if (existente is null)
            {
                return false;
            }

            _context.Invitacions.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}