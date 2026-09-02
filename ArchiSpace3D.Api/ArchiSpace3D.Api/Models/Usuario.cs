using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Models;

[Table("usuario")]
[Index("Email", Name = "usuario_email_key", IsUnique = true)]
[Index("Numerodocumento", Name = "usuario_numerodocumento_key", IsUnique = true)]
public partial class Usuario
{
    [Key]
    [Column("idusuario")]
    public int Idusuario { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("apellido")]
    [StringLength(100)]
    public string Apellido { get; set; } = null!;

    [Column("email")]
    [StringLength(255)]
    public string Email { get; set; } = null!;

    [Column("contrasena")]
    [StringLength(255)]
    public string Contrasena { get; set; } = null!;

    [Column("telefono")]
    [StringLength(20)]
    public string? Telefono { get; set; }

    [Column("direccion")]
    [StringLength(255)]
    public string? Direccion { get; set; }

    [Column("tipodocumento")]
    [StringLength(50)]
    public string? Tipodocumento { get; set; }

    [Column("numerodocumento")]
    [StringLength(50)]
    public string? Numerodocumento { get; set; }

    [Column("rol")]
    [StringLength(20)]
    public string Rol { get; set; } = null!;

    [Column("activo")]
    public bool? Activo { get; set; }

    [Column("tokenrecuperacion")]
    [StringLength(255)]
    public string? Tokenrecuperacion { get; set; }

    [Column("expiraciontoken", TypeName = "timestamp without time zone")]
    public DateTime? Expiraciontoken { get; set; }

    [Column("fecharegistro", TypeName = "timestamp without time zone")]
    public DateTime? Fecharegistro { get; set; }

 
    [JsonIgnore]
    [ValidateNever]
    [InverseProperty("IdarquitectoNavigation")]
    public virtual ICollection<Invitacion> InvitacionIdarquitectoNavigations { get; set; } = new List<Invitacion>();

    [JsonIgnore]
    [ValidateNever]
    [InverseProperty("IdclienteusadoNavigation")]
    public virtual ICollection<Invitacion> InvitacionIdclienteusadoNavigations { get; set; } = new List<Invitacion>();

    [JsonIgnore]
    [ValidateNever]
    [InverseProperty("IdarquitectoNavigation")]
    public virtual ICollection<Proyecto> ProyectoIdarquitectoNavigations { get; set; } = new List<Proyecto>();

    [JsonIgnore]
    [ValidateNever]
    [InverseProperty("IdclienteNavigation")]
    public virtual ICollection<Proyecto> ProyectoIdclienteNavigations { get; set; } = new List<Proyecto>();
}