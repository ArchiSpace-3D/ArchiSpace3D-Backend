import os
import re

file_path = r"C:\Users\angel\OneDrive\Desktop\Archie\Backend\ArchiSpace3D.Api\ArchiSpace3D.Api\Dao\proyectoDAO.cs"
with open(file_path, "r", encoding="utf-8") as f:
    content = f.read()

old_delete = """        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await _context.Proyectos.FirstOrDefaultAsync(p => p.Idproyecto == id);

            if (existente is null)
            {
                return false;
            }

            _context.Proyectos.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }"""

new_delete = """        public async Task<bool> DeleteAsync(int id)
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
        }"""

content = content.replace(old_delete, new_delete)

with open(file_path, "w", encoding="utf-8") as f:
    f.write(content)
