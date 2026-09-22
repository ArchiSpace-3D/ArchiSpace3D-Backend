using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ArchiSpace3D.Api.Models;

[Table("sugerencia")]
public partial class Sugerencia
{
    [Key]
    [Column("idsugerencia")]
    public int Idsugerencia { get; set; }

    [Column("idproyecto")]
    public int Idproyecto { get; set; }

    [Column("idusuario")]
    public int Idusuario { get; set; }

    [Column("titulo")]
    [StringLength(255)]
    public string Titulo { get; set; } = null!;

    [Column("descripcion")]
    public string Descripcion { get; set; } = null!;

    [Column("estado")]
    [StringLength(50)]
    public string? Estado { get; set; }

    [Column("fechacreacion", TypeName = "timestamp without time zone")]
    public DateTime? Fechacreacion { get; set; }

    [JsonIgnore]
    [ValidateNever]
    [ForeignKey("Idproyecto")]
    public virtual Proyecto IdproyectoNavigation { get; set; } = null!;

    [JsonIgnore]
    [ValidateNever]
    [ForeignKey("Idusuario")]
    public virtual Usuario IdusuarioNavigation { get; set; } = null!;
}
