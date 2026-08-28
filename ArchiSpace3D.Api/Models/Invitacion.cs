using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Models;

[Table("invitacion")]
[Index("Codigo", Name = "invitacion_codigo_key", IsUnique = true)]
public partial class Invitacion
{
    [Key]
    [Column("idinvitacion")]
    public int Idinvitacion { get; set; }

    [Column("idproyecto")]
    public int Idproyecto { get; set; }

    [Column("idarquitecto")]
    public int Idarquitecto { get; set; }

    [Column("idclienteusado")]
    public int? Idclienteusado { get; set; }

    [Column("codigo")]
    [StringLength(50)]
    public string Codigo { get; set; } = null!;

    [Column("usada")]
    public bool? Usada { get; set; }

    [Column("fechacreacion", TypeName = "timestamp without time zone")]
    public DateTime? Fechacreacion { get; set; }

    [Column("fechaexpiracion", TypeName = "timestamp without time zone")]
    public DateTime? Fechaexpiracion { get; set; }

    [ForeignKey("Idarquitecto")]
    [InverseProperty("InvitacionIdarquitectoNavigations")]
    public virtual Usuario IdarquitectoNavigation { get; set; } = null!;

    [ForeignKey("Idclienteusado")]
    [InverseProperty("InvitacionIdclienteusadoNavigations")]
    public virtual Usuario? IdclienteusadoNavigation { get; set; }

    [ForeignKey("Idproyecto")]
    [InverseProperty("Invitacions")]
    public virtual Proyecto IdproyectoNavigation { get; set; } = null!;
}
