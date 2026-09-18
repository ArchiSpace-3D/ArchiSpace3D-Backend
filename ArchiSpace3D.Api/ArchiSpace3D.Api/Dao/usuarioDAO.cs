using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Data;
using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Service;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Dao
{
    public class usuarioDao : usuarioDAOImpl
    {
        private readonly ArchiSpaceContext _context;

        public usuarioDao(ArchiSpaceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Idusuario == id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {

            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByNumeroDocumentoAsync(string numeroDocumento)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .AnyAsync(u => u.Numerodocumento == numeroDocumento);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            usuario.Fecharegistro = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            usuario.Activo ??= true;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> UpdateAsync(Usuario usuario)
        {
            var existente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Idusuario == usuario.Idusuario);

            if (existente is null)
            {
                return false;
            }

            

            existente.Nombre = usuario.Nombre;
            existente.Apellido = usuario.Apellido;
            existente.Telefono = usuario.Telefono;
            existente.Direccion = usuario.Direccion;
            existente.Tipodocumento = usuario.Tipodocumento;
            existente.Numerodocumento = usuario.Numerodocumento;

            if (usuario.Avatarurl != null)
            {
                existente.Avatarurl = usuario.Avatarurl;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ActualizarFcmTokenAsync(int idUsuario, string token)
        {
            var existente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Idusuario == idUsuario);

            if (existente is null)
            {
                return false;
            }

            existente.Fcmtoken = token;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Idusuario == id);

            if (existente is null)
            {
                return false;
            }

            
            existente.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
        
    }
}