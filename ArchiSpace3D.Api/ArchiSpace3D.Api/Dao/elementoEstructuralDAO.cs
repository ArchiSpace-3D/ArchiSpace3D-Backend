using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ArchiSpace3D.Api.Dao
{
    public class elementoEstructuralDAO: elementoEstructuralDAOImpl
    {
        private readonly ArchiSpaceContext _context;

        public elementoEstructuralDAO(ArchiSpaceContext context)
        {
            _context = context;
        }
         public async Task<IEnumerable<Elementoestructural>>GetAllAsync()
         {
            return await _context.Elementoestructurals
                 .AsNoTracking()
                 .ToListAsync();
         }
        public async Task<Elementoestructural?> GetByIdAsync(int idElementoEstructural)
        {
            return await _context.Elementoestructurals
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Idelementoestructural == idElementoEstructural);
        }

        public async Task<IEnumerable<Elementoestructural>> GetByVersionDiseñoAsync(int idVersionDiseño)
        {
            return await _context.Elementoestructurals
                .AsNoTracking()
                .Where(e=> e.Idversiondiseno == idVersionDiseño)
                .ToListAsync();
        }

        public async Task<Elementoestructural> CreateAsync(Elementoestructural elemento)
        {
            _context.Elementoestructurals.Add(elemento);
            await _context.SaveChangesAsync();
            return elemento;
        }

        public async Task<bool>UpdateAsync(Elementoestructural elemnto)
        {
            var existente = await _context.Elementoestructurals.FirstOrDefaultAsync(e => e.Idelementoestructural == elemnto.Idelementoestructural);
            if (existente == null)
            {
                return false;
            }

            existente.Tipo = elemnto.Tipo;
            existente.Material = elemnto.Material;
            existente.Posicionx = elemnto.Posicionx;
            existente.Posiciony = elemnto.Posiciony;
            existente.Posicionz = elemnto.Posicionz;
            existente.Dimensionancho = elemnto.Dimensionancho;
            existente.Dimensionalto = elemnto.Dimensionalto;
            existente.Dimensionprofundidad = elemnto.Dimensionprofundidad;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int idElementoEstructural)
        {
            var existente = _context.Elementoestructurals.FirstOrDefault(e => e.Idelementoestructural == idElementoEstructural);
            if (existente is null)
            {
                return false;
            }

            _context.Elementoestructurals.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
