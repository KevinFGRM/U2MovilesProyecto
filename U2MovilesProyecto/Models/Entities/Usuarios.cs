using System;
using System.Collections.Generic;

namespace U2MovilesProyecto.Models.Entities;

public partial class Usuarios
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Accionespartida> Accionespartida { get; set; } = new List<Accionespartida>();

    public virtual ICollection<Amigos> AmigosUsuario1Navigation { get; set; } = new List<Amigos>();

    public virtual ICollection<Amigos> AmigosUsuario2Navigation { get; set; } = new List<Amigos>();

    public virtual ICollection<Mensajes> MensajesEmisorNavigation { get; set; } = new List<Mensajes>();

    public virtual ICollection<Mensajes> MensajesReceptorNavigation { get; set; } = new List<Mensajes>();

    public virtual ICollection<Partidapersonajes> Partidapersonajes { get; set; } = new List<Partidapersonajes>();

    public virtual ICollection<Partidas> PartidasGanadorNavigation { get; set; } = new List<Partidas>();

    public virtual ICollection<Partidas> PartidasJugador1Navigation { get; set; } = new List<Partidas>();

    public virtual ICollection<Partidas> PartidasJugador2Navigation { get; set; } = new List<Partidas>();
}
