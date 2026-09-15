namespace ArchiSpace3D.Api.Models
{
    public class loginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int Idusuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
}