using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Dao
{
    public class modeloImportadoDAO : modeloimportadoDAOImpl
    {
        private readonly ArchiSpaceContext _context;

        public modeloImportadoDAO(ArchiSpaceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Modeloimportado>> GetAllAsync()
        {
            return await _context.Modeloimportados
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Modeloimportado?> GetByIdAsync(int id)
        {
            return await _context.Modeloimportados
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Idmodeloimportado == id);
        }

        public async Task<IEnumerable<Modeloimportado>> GetByVersionDisenoAsync(int idVersionDiseno)
        {
            return await _context.Modeloimportados
                .AsNoTracking()
                .Where(m => m.Idversiondiseno == idVersionDiseno)
                .ToListAsync();
        }

        public async Task<Modeloimportado> CreateAsync(Modeloimportado modelo)
        {
            modelo.Fechaimportacion = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            _context.Modeloimportados.Add(modelo);
            await _context.SaveChangesAsync();
            return modelo;
        }

        // Llamado "UpdateTransformAsync" (no "UpdateAsync" genérico) a propósito:
        // lo único que tiene sentido editar en un modelo ya importado es su
        // posición/rotación/escala dentro de la escena 3D. Nombrearchivo, Formato
        // y Rutastorage pertenecen al archivo físico importado — cambiarlos sería
        // "reimportar", no "editar", así que se dejan fuera de este método.
        public async Task<bool> UpdateTransformAsync(Modeloimportado modelo)
        {
            var existente = await _context.Modeloimportados
                .FirstOrDefaultAsync(m => m.Idmodeloimportado == modelo.Idmodeloimportado);

            if (existente is null)
            {
                return false;
            }

            existente.Posicionx = modelo.Posicionx;
            existente.Posiciony = modelo.Posiciony;
            existente.Posicionz = modelo.Posicionz;
            existente.Rotacionx = modelo.Rotacionx;
            existente.Rotaciony = modelo.Rotaciony;
            existente.Rotacionz = modelo.Rotacionz;
            existente.Escalax = modelo.Escalax;
            existente.Escalay = modelo.Escalay;
            existente.Escalaz = modelo.Escalaz;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await _context.Modeloimportados
                .FirstOrDefaultAsync(m => m.Idmodeloimportado == id);

            if (existente is null)
            {
                return false;
            }

            _context.Modeloimportados.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}