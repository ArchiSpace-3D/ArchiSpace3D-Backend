using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Models;

[Table("modeloimportado")]
public partial class Modeloimportado
{
    [Key]
    [Column("idmodeloimportado")]
    public int Idmodeloimportado { get; set; }

    [Column("idversiondiseno")]
    public int Idversiondiseno { get; set; }

    [Column("nombrearchivo")]
    [StringLength(255)]
    public string Nombrearchivo { get; set; } = null!;

    [Column("formato")]
    [StringLength(10)]
    public string Formato { get; set; } = null!;

    [Column("rutastorage")]
    [StringLength(500)]
    public string Rutastorage { get; set; } = null!;

    [Column("posicionx")]
    [Precision(10, 6)]
    public decimal? Posicionx { get; set; }

    [Column("posiciony")]
    [Precision(10, 6)]
    public decimal? Posiciony { get; set; }

    [Column("posicionz")]
    [Precision(10, 6)]
    public decimal? Posicionz { get; set; }

    [Column("rotacionx")]
    [Precision(10, 6)]
    public decimal? Rotacionx { get; set; }

    [Column("rotaciony")]
    [Precision(10, 6)]
    public decimal? Rotaciony { get; set; }

    [Column("rotacionz")]
    [Precision(10, 6)]
    public decimal? Rotacionz { get; set; }

    [Column("escalax")]
    [Precision(10, 6)]
    public decimal? Escalax { get; set; }

    [Column("escalay")]
    [Precision(10, 6)]
    public decimal? Escalay { get; set; }

    [Column("escalaz")]
    [Precision(10, 6)]
    public decimal? Escalaz { get; set; }

    [Column("fechaimportacion", TypeName = "timestamp without time zone")]
    public DateTime? Fechaimportacion { get; set; }

    [JsonIgnore]
    [ValidateNever]
    [ForeignKey("Idversiondiseno")]
    [InverseProperty("Modeloimportados")]
    public virtual Versiondiseno IdversiondisenoNavigation { get; set; } = null!;
}