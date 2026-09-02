using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Models;

[Table("elementoestructural")]
public partial class Elementoestructural
{
    [Key]
    [Column("idelementoestructural")]
    public int Idelementoestructural { get; set; }

    [Column("idversiondiseno")]
    public int Idversiondiseno { get; set; }

    [Column("tipo")]
    [StringLength(50)]
    public string Tipo { get; set; } = null!;

    [Column("material")]
    [StringLength(100)]
    public string? Material { get; set; }

    [Column("posicionx")]
    [Precision(10, 6)]
    public decimal? Posicionx { get; set; }

    [Column("posiciony")]
    [Precision(10, 6)]
    public decimal? Posiciony { get; set; }

    [Column("posicionz")]
    [Precision(10, 6)]
    public decimal? Posicionz { get; set; }

    [Column("dimensionancho")]
    [Precision(10, 6)]
    public decimal? Dimensionancho { get; set; }

    [Column("dimensionalto")]
    [Precision(10, 6)]
    public decimal? Dimensionalto { get; set; }

    [Column("dimensionprofundidad")]
    [Precision(10, 6)]
    public decimal? Dimensionprofundidad { get; set; }

    [JsonIgnore]
    [ValidateNever]
    [ForeignKey("Idversiondiseno")]
    [InverseProperty("Elementoestructurals")]
    public virtual Versiondiseno IdversiondisenoNavigation { get; set; } = null!;
}