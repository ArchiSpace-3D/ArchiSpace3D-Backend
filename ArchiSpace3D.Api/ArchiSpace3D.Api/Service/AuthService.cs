using ArchiSpace3D.Api.Dao;
using ArchiSpace3D.Api.Models;
using ArchiSpace3D.Api.Util;

namespace ArchiSpace3D.Api.Service
{
    public class AuthService : AuthServiceImpl
    {
        private readonly usuarioDAOImpl _usuarioDao;
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthService(usuarioDAOImpl usuarioDao, JwtTokenGenerator tokenGenerator)
        {
            _usuarioDao = usuarioDao;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<(string Token, Usuario Usuario)> LoginAsync(string email, string contrasena)
        {
            var usuario = await _usuarioDao.GetByEmailAsync(email);

            
            
            if (usuario is null || !BCrypt.Net.BCrypt.Verify(contrasena, usuario.Contrasena))
            {
                throw new InvalidOperationException("Email o contraseña incorrectos.");
            }

            if (usuario.Activo == false)
            {
                throw new InvalidOperationException("Este usuario está inactivo.");
            }

            var token = _tokenGenerator.GenerarToken(usuario);
            return (token, usuario);
        }
    }
}