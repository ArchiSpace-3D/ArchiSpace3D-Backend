using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace ArchiSpace3D.Api.Dao
{
    public class proyectoDAO:proyectoDAOImpl
    {
        private readonly ArchiSpaceContext _context;

        public proyectoDAO(ArchiSpaceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Proyecto>> GetAllAsync()
        {
            return await _context.Proyectos
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<Proyecto?> GetByIdAsync(int id) 
        {
            return await _context.Proyectos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Idproyecto == id);
        }
        public async Task<IEnumerable<Proyecto>> GetByClienteAsync(int id)
        {
            return await _context.Proyectos
                .AsNoTracking()
                .Where(p => p.Idcliente == id)
                .ToListAsync();
        }
        public async Task<IEnumerable<Proyecto>> GetByArquitectoAsync(int id)
        {
            return await _context.Proyectos
                .AsNoTracking()
                .Where(P => P.Idarquitecto == id)
                .ToListAsync();
               
        }

        public async Task<bool> ExistByCodigoSalaActivaAsync(string codigoSala)
        {
            return await _context.Proyectos
            .AsNoTracking()
            .AnyAsync(p => p.Codigosalaactiva == codigoSala);
        }

        public async Task<Proyecto> CreateAsync(Proyecto proyecto)
        {
            proyecto.Fechacreacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            proyecto.Fechaactualizacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.Proyectos.Add(proyecto);
            await _context.SaveChangesAsync();
            return proyecto;

        }
        public async Task<bool> UpdateAsync(Proyecto proyecto)
        {
            var existente = await _context.Proyectos.FirstOrDefaultAsync(p => p.Idproyecto == proyecto.Idproyecto);
            if(existente is null)
            {
                return false;
            }

            existente.Nombre = proyecto.Nombre;
            existente.Descripcion = proyecto.Descripcion;
            existente.Ubicacion = proyecto.Ubicacion;
            existente.Presupuesto = proyecto.Presupuesto;
            existente.Estado = proyecto.Estado;
            existente.Fechaactualizacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await _context.Proyectos.FirstOrDefaultAsync(p => p.Idproyecto == id);

            if (existente is null)
            {
                return false;
            }

            // Eliminar dependencias manualmente para evitar el error de llave foránea (InternalServerError 500)
            
            var mediciones = _context.Medicions.Where(m => m.Idproyecto == id);
            _context.Medicions.RemoveRange(mediciones);

            var espacios = _context.Espaciofisicos.Where(e => e.Idproyecto == id);
            _context.Espaciofisicos.RemoveRange(espacios);

            var notificaciones = _context.Notificacions.Where(n => n.Idproyecto == id);
            _context.Notificacions.RemoveRange(notificaciones);

            var invitaciones = _context.Invitacions.Where(i => i.Idproyecto == id);
            _context.Invitacions.RemoveRange(invitaciones);

            var versiones = await _context.Versiondisenos.Where(v => v.Idproyecto == id).ToListAsync();
            foreach (var v in versiones)
            {
                var elementos = _context.Elementoestructurals.Where(e => e.Idversiondiseno == v.Idversiondiseno);
                _context.Elementoestructurals.RemoveRange(elementos);

                var modelos = _context.Modeloimportados.Where(m => m.Idversiondiseno == v.Idversiondiseno);
                _context.Modeloimportados.RemoveRange(modelos);
            }
            _context.Versiondisenos.RemoveRange(versiones);

            _context.Proyectos.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }

        }

    }

