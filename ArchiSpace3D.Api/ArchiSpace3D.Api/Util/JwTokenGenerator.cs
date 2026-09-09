using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArchiSpace3D.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace ArchiSpace3D.Api.Util
{
    // Clase de utilidad dedicada solo a construir el JWT -- separada del
    // AuthService a propósito, para no mezclar "cómo se arma un token"
    // (detalle técnico) con "qué reglas de negocio tiene el login"
    // (email/contraseña correctos, usuario activo, etc.).
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerarToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims = la información que viaja DENTRO del token, firmada.
            // ClaimTypes.Role es el que .NET usa automáticamente para que
            // [Authorize(Roles = "Arquitecto")] funcione sin configuración extra.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Idusuario.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}")
            };

            var expireMinutes = double.Parse(jwtSettings["ExpireMinutes"] ?? "120");

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}