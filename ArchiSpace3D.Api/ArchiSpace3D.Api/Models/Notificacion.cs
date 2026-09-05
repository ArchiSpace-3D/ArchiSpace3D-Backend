using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Models;

[Table("notificacion")]
public partial class Notificacion
{
    [Key]
    [Column("idnotificacion")]
    public int Idnotificacion { get; set; }

    [Column("idproyecto")]
    public int Idproyecto { get; set; }

    [Column("idversiondiseno")]
    public int? Idversiondiseno { get; set; }

    [Column("tipo")]
    [StringLength(50)]
    public string? Tipo { get; set; }

    [Column("mensaje")]
    public string Mensaje { get; set; } = null!;

    [Column("leida")]
    public bool? Leida { get; set; }

    [Column("fechaenvio", TypeName = "timestamp without time zone")]
    public DateTime? Fechaenvio { get; set; }

    [JsonIgnore]
    [ValidateNever]
    [ForeignKey("Idproyecto")]
    [InverseProperty("Notificacions")]
    public virtual Proyecto IdproyectoNavigation { get; set; } = null!;

    [JsonIgnore]
    [ValidateNever]
    [ForeignKey("Idversiondiseno")]
    [InverseProperty("Notificacions")]
    public virtual Versiondiseno? IdversiondisenoNavigation { get; set; }
}