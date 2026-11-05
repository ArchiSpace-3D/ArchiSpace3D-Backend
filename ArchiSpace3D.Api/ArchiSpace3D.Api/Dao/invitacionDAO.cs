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
            if (string.IsNullOrWhiteSpace(codigo)) return null;
            var norm = codigo.Trim().ToUpperInvariant();
            var inv = await _context.Invitacions
                .FirstOrDefaultAsync(i => i.Codigo.ToUpper() == norm);

            if (inv != null) return inv;

            // Compatibilidad con salas activadas con código SALA-XXXXXX sin fila en invitacion
            var proy = await _context.Proyectos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Codigosalaactiva != null && p.Codigosalaactiva.ToUpper() == norm);

            if (proy != null && proy.Idarquitecto.HasValue && proy.Idarquitecto.Value > 0)
            {
                var autoInv = new Invitacion
                {
                    Idproyecto = proy.Idproyecto,
                    Idarquitecto = proy.Idarquitecto.Value,
                    Codigo = proy.Codigosalaactiva!,
                    Fechacreacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
                    Usada = false
                };
                _context.Invitacions.Add(autoInv);
                await _context.SaveChangesAsync();
                return autoInv;
            }

            return null;
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
            if (string.IsNullOrWhiteSpace(codigo)) return false;
            var norm = codigo.Trim().ToUpperInvariant();
            return await _context.Invitacions
                .AsNoTracking()
                .AnyAsync(i => i.Codigo.ToUpper() == norm);
        }

        public async Task<Invitacion> CreateAsync(Invitacion invitacion)
        {
            invitacion.Codigo = invitacion.Codigo.Trim().ToUpperInvariant();
            invitacion.Fechacreacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            if (invitacion.Fechaexpiracion.HasValue)
            {
                invitacion.Fechaexpiracion = DateTime.SpecifyKind(invitacion.Fechaexpiracion.Value, DateTimeKind.Unspecified);
            }

            invitacion.Usada ??= false;

            _context.Invitacions.Add(invitacion);

            var proyecto = await _context.Proyectos.FirstOrDefaultAsync(p => p.Idproyecto == invitacion.Idproyecto);
            if (proyecto != null)
            {
                proyecto.Codigosalaactiva = invitacion.Codigo;
                proyecto.Fechaaperturasala ??= invitacion.Fechacreacion;
                proyecto.Fechaactualizacion = invitacion.Fechacreacion;
            }

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

            var proyecto = await _context.Proyectos
                .FirstOrDefaultAsync(p => p.Idproyecto == existente.Idproyecto);

            if (proyecto != null)
            {
                proyecto.Idcliente = idClienteUsado;
                proyecto.Fechaactualizacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            }

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
