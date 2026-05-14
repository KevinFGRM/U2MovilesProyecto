using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using U2MovilesProyecto.Models.Entities;

namespace U2MovilesProyecto.Data;

public partial class MiniJokeRpgContext : DbContext
{
    public MiniJokeRpgContext()
    {
    }

    public MiniJokeRpgContext(DbContextOptions<MiniJokeRpgContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accionespartida> Accionespartida { get; set; }

    public virtual DbSet<Amigos> Amigos { get; set; }

    public virtual DbSet<Habilidades> Habilidades { get; set; }

    public virtual DbSet<Mensajes> Mensajes { get; set; }

    public virtual DbSet<Partidapersonajes> Partidapersonajes { get; set; }

    public virtual DbSet<Partidas> Partidas { get; set; }

    public virtual DbSet<Personajes> Personajes { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Accionespartida>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PRIMARY");

            entity.ToTable("accionespartida");

            entity.HasIndex(e => e.Habilidad, "Habilidad");

            entity.HasIndex(e => e.IdPartida, "IdPartida");

            entity.HasIndex(e => e.Usuario, "Usuario");

            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.HabilidadNavigation).WithMany(p => p.Accionespartida)
                .HasForeignKey(d => d.Habilidad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("accionespartida_ibfk_3");

            entity.HasOne(d => d.IdPartidaNavigation).WithMany(p => p.Accionespartida)
                .HasForeignKey(d => d.IdPartida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("accionespartida_ibfk_1");

            entity.HasOne(d => d.UsuarioNavigation).WithMany(p => p.Accionespartida)
                .HasForeignKey(d => d.Usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("accionespartida_ibfk_2");
        });

        modelBuilder.Entity<Amigos>(entity =>
        {
            entity.HasKey(e => e.IdRelacion).HasName("PRIMARY");

            entity.ToTable("amigos");

            entity.HasIndex(e => e.Usuario1, "Usuario1");

            entity.HasIndex(e => e.Usuario2, "Usuario2");

            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'pendiente'")
                .HasColumnType("enum('pendiente','aceptado','rechazado')");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Usuario1Navigation).WithMany(p => p.AmigosUsuario1Navigation)
                .HasForeignKey(d => d.Usuario1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("amigos_ibfk_1");

            entity.HasOne(d => d.Usuario2Navigation).WithMany(p => p.AmigosUsuario2Navigation)
                .HasForeignKey(d => d.Usuario2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("amigos_ibfk_2");
        });

        modelBuilder.Entity<Habilidades>(entity =>
        {
            entity.HasKey(e => e.IdHabilidad).HasName("PRIMARY");

            entity.ToTable("habilidades");

            entity.HasIndex(e => e.IdPersonaje, "IdPersonaje");

            entity.Property(e => e.CostoMana).HasDefaultValueSql("'0'");
            entity.Property(e => e.Curacion).HasDefaultValueSql("'0'");
            entity.Property(e => e.Dano).HasDefaultValueSql("'0'");
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.Nombre).HasMaxLength(50);

            entity.HasOne(d => d.IdPersonajeNavigation).WithMany(p => p.Habilidades)
                .HasForeignKey(d => d.IdPersonaje)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("habilidades_ibfk_1");
        });

        modelBuilder.Entity<Mensajes>(entity =>
        {
            entity.HasKey(e => e.IdMensaje).HasName("PRIMARY");

            entity.ToTable("mensajes");

            entity.HasIndex(e => e.Emisor, "Emisor");

            entity.HasIndex(e => e.Receptor, "Receptor");

            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Mensaje).HasColumnType("text");

            entity.HasOne(d => d.EmisorNavigation).WithMany(p => p.MensajesEmisorNavigation)
                .HasForeignKey(d => d.Emisor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mensajes_ibfk_1");

            entity.HasOne(d => d.ReceptorNavigation).WithMany(p => p.MensajesReceptorNavigation)
                .HasForeignKey(d => d.Receptor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mensajes_ibfk_2");
        });

        modelBuilder.Entity<Partidapersonajes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("partidapersonajes");

            entity.HasIndex(e => e.IdPartida, "IdPartida");

            entity.HasIndex(e => e.IdPersonaje, "IdPersonaje");

            entity.HasIndex(e => e.IdUsuario, "IdUsuario");

            entity.HasOne(d => d.IdPartidaNavigation).WithMany(p => p.Partidapersonajes)
                .HasForeignKey(d => d.IdPartida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partidapersonajes_ibfk_1");

            entity.HasOne(d => d.IdPersonajeNavigation).WithMany(p => p.Partidapersonajes)
                .HasForeignKey(d => d.IdPersonaje)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partidapersonajes_ibfk_3");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Partidapersonajes)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partidapersonajes_ibfk_2");
        });

        modelBuilder.Entity<Partidas>(entity =>
        {
            entity.HasKey(e => e.IdPartida).HasName("PRIMARY");

            entity.ToTable("partidas");

            entity.HasIndex(e => e.Ganador, "Ganador");

            entity.HasIndex(e => e.Jugador1, "Jugador1");

            entity.HasIndex(e => e.Jugador2, "Jugador2");

            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'activa'")
                .HasColumnType("enum('activa','finalizada')");
            entity.Property(e => e.FechaInicio)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.GanadorNavigation).WithMany(p => p.PartidasGanadorNavigation)
                .HasForeignKey(d => d.Ganador)
                .HasConstraintName("partidas_ibfk_3");

            entity.HasOne(d => d.Jugador1Navigation).WithMany(p => p.PartidasJugador1Navigation)
                .HasForeignKey(d => d.Jugador1)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partidas_ibfk_1");

            entity.HasOne(d => d.Jugador2Navigation).WithMany(p => p.PartidasJugador2Navigation)
                .HasForeignKey(d => d.Jugador2)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partidas_ibfk_2");
        });

        modelBuilder.Entity<Personajes>(entity =>
        {
            entity.HasKey(e => e.IdPersonaje).HasName("PRIMARY");

            entity.ToTable("personajes");

            entity.Property(e => e.Nombre).HasMaxLength(30);
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Correo, "Correo").IsUnique();

            entity.HasIndex(e => e.NombreUsuario, "NombreUsuario").IsUnique();

            entity.Property(e => e.ContrasenaHash).HasMaxLength(255);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreUsuario).HasMaxLength(30);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
