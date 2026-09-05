using ArchiSpace3D.Api.Models;

namespace ArchiSpace3D.Api.Service
{
    public interface AuthServiceImpl
    {
        Task<(string Token, Usuario Usuario)> LoginAsync(string email, string contrasena);
    }
}
