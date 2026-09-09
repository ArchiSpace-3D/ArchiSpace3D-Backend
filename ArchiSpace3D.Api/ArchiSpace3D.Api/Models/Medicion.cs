using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Models;

[Table("medicion")]
public partial class Medicion
{
    [Key]
    [Column("idmedicion")]
    public int Idmedicion { get; set; }

    [Column("idproyecto")]
    public int Idproyecto { get; set; }

    [Column("puntoinicial", TypeName = "jsonb")]
    public string Puntoinicial { get; set; } = null!;

    [Column("puntofinal", TypeName = "jsonb")]
    public string Puntofinal { get; set; } = null!;

    [Column("distancia")]
    [Precision(10, 3)]
    public decimal? Distancia { get; set; }

    [Column("fechamedicion", TypeName = "timestamp without time zone")]
    public DateTime? Fechamedicion { get; set; }

    [JsonIgnore]
    [ValidateNever]
    [ForeignKey("Idproyecto")]
    [InverseProperty("Medicions")]
    public virtual Proyecto IdproyectoNavigation { get; set; } = null!;
}