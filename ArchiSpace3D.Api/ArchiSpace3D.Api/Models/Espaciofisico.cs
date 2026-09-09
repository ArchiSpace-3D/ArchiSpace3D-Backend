using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Models;

[Table("espaciofisico")]
[Index("Idproyecto", Name = "espaciofisico_idproyecto_key", IsUnique = true)]
public partial class Espaciofisico
{
    [Key]
    [Column("idespaciofisico")]
    public int Idespaciofisico { get; set; }

    [Column("idproyecto")]
    public int Idproyecto { get; set; }

    [Column("descripcion")]
    [StringLength(255)]
    public string? Descripcion { get; set; }

    [Column("anchoaproximado")]
    [Precision(10, 2)]
    public decimal? Anchoaproximado { get; set; }

    [Column("largoaproximado")]
    [Precision(10, 2)]
    public decimal? Largoaproximado { get; set; }

    [Column("altoaproximado")]
    [Precision(10, 2)]
    public decimal? Altoaproximado { get; set; }

    [Column("puntosreferencia", TypeName = "jsonb")]
    public string Puntosreferencia { get; set; } = null!;

    [Column("orientacionazimuth")]
    [Precision(10, 2)]
    public decimal? Orientacionazimuth { get; set; }

    [Column("fechacaptura", TypeName = "timestamp without time zone")]
    public DateTime? Fechacaptura { get; set; }

    [JsonIgnore]
    [ValidateNever]
    [ForeignKey("Idproyecto")]
    [InverseProperty("Espaciofisico")]
    public virtual Proyecto IdproyectoNavigation { get; set; } = null!;
}