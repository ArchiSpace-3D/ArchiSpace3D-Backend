using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Dao
{
   
    public class medicionDAO : medicionDAOImpl
    {
        private readonly ArchiSpaceContext _context;

        public medicionDAO(ArchiSpaceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Medicion>> GetAllAsync()
        {
            return await _context.Medicions
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Medicion?> GetByIdAsync(int id)
        {
            return await _context.Medicions
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Idmedicion == id);
        }

        public async Task<IEnumerable<Medicion>> GetByProyectoAsync(int idProyecto)
        {
            return await _context.Medicions
                .AsNoTracking()
                .Where(m => m.Idproyecto == idProyecto)
                .ToListAsync();
        }

        public async Task<Medicion> CreateAsync(Medicion medicion)
        {
            medicion.Fechamedicion ??= DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            _context.Medicions.Add(medicion);
            await _context.SaveChangesAsync();
            return medicion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await _context.Medicions
                .FirstOrDefaultAsync(m => m.Idmedicion == id);

            if (existente is null)
            {
                return false;
            }

            _context.Medicions.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}