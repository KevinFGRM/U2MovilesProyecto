using System;
using System.Collections.Generic;

namespace MiniJokeRPGAPI.Models.Entities;

public partial class Partidas
{
    public int IdPartida { get; set; }

    public int Jugador1 { get; set; }

    public int Jugador2 { get; set; }

    public int? Ganador { get; set; }

    public int TurnoActual { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaInicio { get; set; }

    public virtual ICollection<Accionespartida> Accionespartida { get; set; } = new List<Accionespartida>();

    public virtual Usuarios? GanadorNavigation { get; set; }

    public virtual Usuarios Jugador1Navigation { get; set; } = null!;

    public virtual Usuarios Jugador2Navigation { get; set; } = null!;

    public virtual ICollection<Partidahabilidades> Partidahabilidades { get; set; } = new List<Partidahabilidades>();

    public virtual ICollection<Partidapersonajes> Partidapersonajes { get; set; } = new List<Partidapersonajes>();
}
