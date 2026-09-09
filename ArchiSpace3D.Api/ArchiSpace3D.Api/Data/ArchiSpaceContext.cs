using System;
using System.Collections.Generic;
using ArchiSpace3D.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchiSpace3D.Api.Data;

public partial class ArchiSpaceContext : DbContext
{
    public ArchiSpaceContext(DbContextOptions<ArchiSpaceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Elementoestructural> Elementoestructurals { get; set; }

    public virtual DbSet<Espaciofisico> Espaciofisicos { get; set; }

    public virtual DbSet<Invitacion> Invitacions { get; set; }

    public virtual DbSet<Medicion> Medicions { get; set; }

    public virtual DbSet<Modeloimportado> Modeloimportados { get; set; }

    public virtual DbSet<Notificacion> Notificacions { get; set; }

    public virtual DbSet<Proyecto> Proyectos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Versiondiseno> Versiondisenos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Elementoestructural>(entity =>
        {
            entity.HasKey(e => e.Idelementoestructural).HasName("elementoestructural_pkey");

            entity.Property(e => e.Dimensionalto).HasDefaultValue(0m);
            entity.Property(e => e.Dimensionancho).HasDefaultValue(0m);
            entity.Property(e => e.Dimensionprofundidad).HasDefaultValue(0m);
            entity.Property(e => e.Posicionx).HasDefaultValue(0m);
            entity.Property(e => e.Posiciony).HasDefaultValue(0m);
            entity.Property(e => e.Posicionz).HasDefaultValue(0m);

            entity.HasOne(d => d.IdversiondisenoNavigation).WithMany(p => p.Elementoestructurals)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("elementoestructural_idversiondiseno_fkey");
        });

        modelBuilder.Entity<Espaciofisico>(entity =>
        {
            entity.HasKey(e => e.Idespaciofisico).HasName("espaciofisico_pkey");

            entity.Property(e => e.Fechacaptura).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Puntosreferencia).HasDefaultValueSql("'[]'::jsonb");

            entity.HasOne(d => d.IdproyectoNavigation).WithOne(p => p.Espaciofisico)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("espaciofisico_idproyecto_fkey");
        });

        modelBuilder.Entity<Invitacion>(entity =>
        {
            entity.HasKey(e => e.Idinvitacion).HasName("invitacion_pkey");

            entity.Property(e => e.Fechacreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Usada).HasDefaultValue(false);

            entity.HasOne(d => d.IdarquitectoNavigation).WithMany(p => p.InvitacionIdarquitectoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invitacion_idarquitecto_fkey");

            entity.HasOne(d => d.IdclienteusadoNavigation).WithMany(p => p.InvitacionIdclienteusadoNavigations).HasConstraintName("invitacion_idclienteusado_fkey");

            entity.HasOne(d => d.IdproyectoNavigation).WithMany(p => p.Invitacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("invitacion_idproyecto_fkey");
        });

        modelBuilder.Entity<Medicion>(entity =>
        {
            entity.HasKey(e => e.Idmedicion).HasName("medicion_pkey");

            entity.Property(e => e.Fechamedicion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.IdproyectoNavigation).WithMany(p => p.Medicions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("medicion_idproyecto_fkey");
        });

        modelBuilder.Entity<Modeloimportado>(entity =>
        {
            entity.HasKey(e => e.Idmodeloimportado).HasName("modeloimportado_pkey");

            entity.Property(e => e.Escalax).HasDefaultValue(1m);
            entity.Property(e => e.Escalay).HasDefaultValue(1m);
            entity.Property(e => e.Escalaz).HasDefaultValue(1m);
            entity.Property(e => e.Fechaimportacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Posicionx).HasDefaultValue(0m);
            entity.Property(e => e.Posiciony).HasDefaultValue(0m);
            entity.Property(e => e.Posicionz).HasDefaultValue(0m);
            entity.Property(e => e.Rotacionx).HasDefaultValue(0m);
            entity.Property(e => e.Rotaciony).HasDefaultValue(0m);
            entity.Property(e => e.Rotacionz).HasDefaultValue(0m);

            entity.HasOne(d => d.IdversiondisenoNavigation).WithMany(p => p.Modeloimportados)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("modeloimportado_idversiondiseno_fkey");
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.Idnotificacion).HasName("notificacion_pkey");

            entity.Property(e => e.Fechaenvio).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Leida).HasDefaultValue(false);

            entity.HasOne(d => d.IdproyectoNavigation).WithMany(p => p.Notificacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notificacion_idproyecto_fkey");

            entity.HasOne(d => d.IdversiondisenoNavigation).WithMany(p => p.Notificacions).HasConstraintName("notificacion_idversiondiseno_fkey");
        });

        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.Idproyecto).HasName("proyecto_pkey");

            entity.Property(e => e.Estado).HasDefaultValueSql("'activo'::character varying");
            entity.Property(e => e.Fechaactualizacion).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Fechacreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.IdarquitectoNavigation).WithMany(p => p.ProyectoIdarquitectoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proyecto_idarquitecto_fkey");

            entity.HasOne(d => d.IdclienteNavigation).WithMany(p => p.ProyectoIdclienteNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proyecto_idcliente_fkey");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Idusuario).HasName("usuario_pkey");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Fecharegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Versiondiseno>(entity =>
        {
            entity.HasKey(e => e.Idversiondiseno).HasName("versiondiseno_pkey");

            entity.Property(e => e.Esactual).HasDefaultValue(false);
            entity.Property(e => e.Fechacreacion).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.IdproyectoNavigation).WithMany(p => p.Versiondisenos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("versiondiseno_idproyecto_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
