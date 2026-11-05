using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Dao
{
    public class espacioFisicoDAO : espacioFisicoDAOImpl
    {
        private readonly ArchiSpaceContext _context;

        public espacioFisicoDAO(ArchiSpaceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Espaciofisico>> GetAllAsync()
        {
            return await _context.Espaciofisicos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Espaciofisico?> GetByIdAsync(int id)
        {
            return await _context.Espaciofisicos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Idespaciofisico == id);
        }

        public async Task<Espaciofisico?> GetByProyectoAsync(int idProyecto)
        {
            var todos = await _context.Espaciofisicos
                .AsNoTracking()
                .Where(e => e.Idproyecto == idProyecto)
                .ToListAsync();

            return todos.FirstOrDefault(e => e.Descripcion == "__PLANO_CAD__")
                ?? todos.FirstOrDefault(e => !string.IsNullOrWhiteSpace(e.Puntosreferencia) && e.Puntosreferencia.TrimStart().StartsWith("{"))
                ?? todos.FirstOrDefault();
        }

        public async Task<IEnumerable<Espaciofisico>> GetListByProyectoAsync(int idProyecto)
        {
            return await _context.Espaciofisicos
                .AsNoTracking()
                .Where(e => e.Idproyecto == idProyecto && e.Descripcion != "__PLANO_CAD__")
                .OrderByDescending(e => e.Idespaciofisico)
                .ToListAsync();
        }

        public async Task<bool> ExistsByProyectoAsync(int idProyecto)
        {
            return await _context.Espaciofisicos
                .AsNoTracking()
                .AnyAsync(e => e.Idproyecto == idProyecto);
        }

        public async Task<Espaciofisico> CreateAsync(Espaciofisico espacio)
        {
            espacio.Fechacaptura = espacio.Fechacaptura.HasValue
                ? DateTime.SpecifyKind(espacio.Fechacaptura.Value, DateTimeKind.Unspecified)
                : DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            if (string.IsNullOrWhiteSpace(espacio.Puntosreferencia))
                espacio.Puntosreferencia = "[]";

            _context.Espaciofisicos.Add(espacio);
            await _context.SaveChangesAsync();
            return espacio;
        }

        public async Task<bool> UpdateAsync(Espaciofisico espacio)
        {
            var existente = await _context.Espaciofisicos
                .FirstOrDefaultAsync(e => e.Idespaciofisico == espacio.Idespaciofisico);

            if (existente is null)
            {
                return false;
            }

            existente.Descripcion = espacio.Descripcion;
            existente.Anchoaproximado = espacio.Anchoaproximado;
            existente.Largoaproximado = espacio.Largoaproximado;
            existente.Altoaproximado = espacio.Altoaproximado;
            if (!string.IsNullOrWhiteSpace(espacio.Puntosreferencia))
                existente.Puntosreferencia = espacio.Puntosreferencia;
            existente.Orientacionazimuth = espacio.Orientacionazimuth;
            if (espacio.Fechacaptura.HasValue)
                existente.Fechacaptura = DateTime.SpecifyKind(espacio.Fechacaptura.Value, DateTimeKind.Unspecified);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await _context.Espaciofisicos
                .FirstOrDefaultAsync(e => e.Idespaciofisico == id);

            if (existente is null)
            {
                return false;
            }

            _context.Espaciofisicos.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
