using System.ComponentModel.DataAnnotations;

namespace ArchiSpace3D.Api.Models
{
    public class ActualizarUsuarioDto
    {
        [Required]
        public int Idusuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = null!;

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(255)]
        public string? Direccion { get; set; }

        [StringLength(50)]
        public string? Tipodocumento { get; set; }

        [StringLength(50)]
        public string? Numerodocumento { get; set; }

        [StringLength(1024)]
        public string? Avatarurl { get; set; }
    }
}
