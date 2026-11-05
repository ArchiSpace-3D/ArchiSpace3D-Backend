using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Dao
{
    public class versiondisenoDAO : versiondiseñoDAOImpl
    {
        private readonly ArchiSpaceContext _context;

        public versiondisenoDAO(ArchiSpaceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Versiondiseno>> GetAllAsync()
        {
            return await _context.Versiondisenos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Versiondiseno?> GetByIdAsync(int id)
        {
            return await _context.Versiondisenos
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Idversiondiseno == id);
        }

        public async Task<IEnumerable<Versiondiseno>> GetByProyectoAsync(int idProyecto)
        {
            return await _context.Versiondisenos
                .AsNoTracking()
                .Where(v => v.Idproyecto == idProyecto)
                .OrderByDescending(v => v.Numeroversion)
                .ToListAsync();
        }

        public async Task<Versiondiseno?> GetVersionActualAsync(int idProyecto)
        {
            return await _context.Versiondisenos
                .AsNoTracking()
                .Where(v => v.Idproyecto == idProyecto && v.Esactual == true)
                .OrderByDescending(v => v.Numeroversion)
                .ThenByDescending(v => v.Idversiondiseno)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsNumeroVersionAsync(int idProyecto, int numeroVersion)
        {
            return await _context.Versiondisenos
                .AsNoTracking()
                .AnyAsync(v => v.Idproyecto == idProyecto && v.Numeroversion == numeroVersion);
        }

        public async Task<Versiondiseno> CreateAsync(Versiondiseno version)
        {
            if (version.Esactual == true)
            {
                var anteriores = await _context.Versiondisenos
                    .Where(v => v.Idproyecto == version.Idproyecto && v.Esactual == true)
                    .ToListAsync();
                foreach (var ant in anteriores)
                {
                    ant.Esactual = false;
                }
            }

            version.Fechacreacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            if (string.IsNullOrWhiteSpace(version.Puntosreferencia))
                version.Puntosreferencia = "{}";

            _context.Versiondisenos.Add(version);
            await _context.SaveChangesAsync();
            return version;
        }

        public async Task<bool> UpdateAsync(Versiondiseno version)
        {
            var existente = await _context.Versiondisenos
                .FirstOrDefaultAsync(v => v.Idversiondiseno == version.Idversiondiseno);

            if (existente is null) return false;

            if (!string.IsNullOrWhiteSpace(version.Puntosreferencia))
                existente.Puntosreferencia = version.Puntosreferencia;

            if (version.Esactual.HasValue)
            {
                if (version.Esactual.Value)
                {
                    var otras = await _context.Versiondisenos
                        .Where(v => v.Idproyecto == existente.Idproyecto && v.Idversiondiseno != existente.Idversiondiseno)
                        .ToListAsync();
                    foreach (var otra in otras) otra.Esactual = false;
                }
                existente.Esactual = version.Esactual;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarcarComoActualAsync(int id, int idProyecto)
        {
            var versionesDelProyecto = await _context.Versiondisenos
                .Where(v => v.Idproyecto == idProyecto)
                .ToListAsync();

            var objetivo = versionesDelProyecto.FirstOrDefault(v => v.Idversiondiseno == id);
            if (objetivo is null)
            {
                return false;
            }

            foreach (var version in versionesDelProyecto)
            {
                version.Esactual = version.Idversiondiseno == id;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await _context.Versiondisenos
                .FirstOrDefaultAsync(v => v.Idversiondiseno == id);

            if (existente is null)
            {
                return false;
            }

            var elementos = _context.Elementoestructurals.Where(e => e.Idversiondiseno == id);
            _context.Elementoestructurals.RemoveRange(elementos);

            var modelos = _context.Modeloimportados.Where(m => m.Idversiondiseno == id);
            _context.Modeloimportados.RemoveRange(modelos);

            var notificaciones = await _context.Notificacions
                .Where(n => n.Idversiondiseno == id)
                .ToListAsync();
            foreach (var notif in notificaciones)
            {
                notif.Idversiondiseno = null;
            }

            _context.Versiondisenos.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
