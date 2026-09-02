using System.Security.Claims;

namespace ArchiSpace3D.Api.Util
{
    public static class ClaimsPrincipalExtensions
    {
        // El NameIdentifier lo puso JwtTokenGenerator = usuario.Idusuario.ToString()
        public static int GetIdUsuario(this ClaimsPrincipal user)
        {
            var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(raw!);
        }

        public static string GetRol(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Role)!;
        }
    }
}