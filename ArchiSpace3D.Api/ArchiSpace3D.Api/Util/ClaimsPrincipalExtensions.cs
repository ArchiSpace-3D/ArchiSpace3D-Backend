using System.Security.Claims;

namespace ArchiSpace3D.Api.Util
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetIdUsuario(this ClaimsPrincipal user)
        {
            var raw = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var id) ? id : 0;
        }

        public static string GetRol(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }
    }
}
