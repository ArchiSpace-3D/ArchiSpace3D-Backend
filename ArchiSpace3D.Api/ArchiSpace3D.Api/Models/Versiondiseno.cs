using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Models;

[Table("versiondiseno")]
[Index("Idproyecto", "Numeroversion", Name = "versiondiseno_idproyecto_numeroversion_key", IsUnique = true)]
public partial class Versiondiseno
{
    [Key]
    [Column("idversiondiseno")]
    public int Idversiondiseno { get; set; }

    [Column("idproyecto")]
    public int Idproyecto { get; set; }

    [Column("numeroversion")]
    public int Numeroversion { get; set; }

    [Column("esactual")]
    public bool? Esactual { get; set; }

    [Column("fechacreacion", TypeName = "timestamp without time zone")]
    public DateTime? Fechacreacion { get; set; }

    [JsonIgnore]
    [ValidateNever]
    [InverseProperty("IdversiondisenoNavigation")]
    public virtual ICollection<Elementoestructural> Elementoestructurals { get; set; } = new List<Elementoestructural>();

    [JsonIgnore]
    [ValidateNever]
    [ForeignKey("Idproyecto")]
    [InverseProperty("Versiondisenos")]
    public virtual Proyecto IdproyectoNavigation { get; set; } = null!;

    [JsonIgnore]
    [ValidateNever]
    [InverseProperty("IdversiondisenoNavigation")]
    public virtual ICollection<Modeloimportado> Modeloimportados { get; set; } = new List<Modeloimportado>();

    [JsonIgnore]
    [ValidateNever]
    [InverseProperty("IdversiondisenoNavigation")]
    public virtual ICollection<Notificacion> Notificacions { get; set; } = new List<Notificacion>();
}