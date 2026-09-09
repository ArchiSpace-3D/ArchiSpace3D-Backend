using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Dao
{
    
    public class notificacionDAO : notificacionDAOImpl
    {
        private readonly ArchiSpaceContext _context;

        public notificacionDAO(ArchiSpaceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notificacion>> GetAllAsync()
        {
            return await _context.Notificacions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Notificacion?> GetByIdAsync(int id)
        {
            return await _context.Notificacions
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Idnotificacion == id);
        }

        public async Task<IEnumerable<Notificacion>> GetByProyectoAsync(int idProyecto)
        {
            return await _context.Notificacions
                .AsNoTracking()
                .Where(n => n.Idproyecto == idProyecto)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notificacion>> GetNoLeidasByProyectoAsync(int idProyecto)
        {
            return await _context.Notificacions
                .AsNoTracking()
                .Where(n => n.Idproyecto == idProyecto && n.Leida != true)
                .ToListAsync();
        }

        public async Task<Notificacion> CreateAsync(Notificacion notificacion)
        {
            notificacion.Fechaenvio = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            notificacion.Leida ??= false;

            _context.Notificacions.Add(notificacion);
            await _context.SaveChangesAsync();
            return notificacion;
        }

        public async Task<bool> MarcarComoLeidaAsync(int id)
        {
            var existente = await _context.Notificacions
                .FirstOrDefaultAsync(n => n.Idnotificacion == id);

            if (existente is null)
            {
                return false;
            }

            existente.Leida = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await _context.Notificacions
                .FirstOrDefaultAsync(n => n.Idnotificacion == id);

            if (existente is null)
            {
                return false;
            }

            _context.Notificacions.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}