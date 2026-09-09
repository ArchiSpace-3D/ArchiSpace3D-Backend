using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Models;

[Table("proyecto")]
[Index("Codigosalaactiva", Name = "proyecto_codigosalaactiva_key", IsUnique = true)]
public partial class Proyecto
{
    [Key]
    [Column("idproyecto")]
    public int Idproyecto { get; set; }

    [Column("idarquitecto")]
    public int Idarquitecto { get; set; }

    [Column("idcliente")]
    public int Idcliente { get; set; }

    [Column("nombre")]
    [StringLength(255)]
    public string Nombre { get; set; } = null!;

    [Column("descripcion")]
    public string? Descripcion { get; set; }

    [Column("ubicacion")]
    [StringLength(255)]
    public string? Ubicacion { get; set; }

    [Column("estado")]
    [StringLength(50)]
    public string? Estado { get; set; }

    [Column("presupuesto")]
    [Precision(15, 2)]
    public decimal? Presupuesto { get; set; }

    [Column("codigosalaactiva")]
    [StringLength(20)]
    public string? Codigosalaactiva { get; set; }

    [Column("fechaaperturasala", TypeName = "timestamp without time zone")]
    public DateTime? Fechaaperturasala { get; set; }

    [Column("fechaactualizacion", TypeName = "timestamp without time zone")]
    public DateTime? Fechaactualizacion { get; set; }

    [Column("fechavisualizacion", TypeName = "timestamp without time zone")]
    public DateTime? Fechavisualizacion { get; set; }

    [Column("fechacreacion", TypeName = "timestamp without time zone")]
    public DateTime? Fechacreacion { get; set; }

    [InverseProperty("IdproyectoNavigation")]
    public virtual Espaciofisico? Espaciofisico { get; set; }

    [ForeignKey("Idarquitecto")]
    [InverseProperty("ProyectoIdarquitectoNavigations")]
    public virtual Usuario IdarquitectoNavigation { get; set; } = null!;

    [ForeignKey("Idcliente")]
    [InverseProperty("ProyectoIdclienteNavigations")]
    public virtual Usuario IdclienteNavigation { get; set; } = null!;

    [InverseProperty("IdproyectoNavigation")]
    public virtual ICollection<Invitacion> Invitacions { get; set; } = new List<Invitacion>();

    [InverseProperty("IdproyectoNavigation")]
    public virtual ICollection<Medicion> Medicions { get; set; } = new List<Medicion>();

    [InverseProperty("IdproyectoNavigation")]
    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();

    [InverseProperty("IdproyectoNavigation")]
    public virtual ICollection<Versiondiseno> Versiondisenos { get; set; } = new List<Versiondiseno>();
}
