using System;
using System.Collections.Generic;

namespace MiniJokeRPGAPI.Models.Entities;

public partial class Habilidades
{
    public int IdHabilidad { get; set; }

    public int IdPersonaje { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int? CostoMana { get; set; }

    public int? Dano { get; set; }

    public int? Curacion { get; set; }

    public virtual ICollection<Accionespartida> Accionespartida { get; set; } = new List<Accionespartida>();

    public virtual Personajes IdPersonajeNavigation { get; set; } = null!;

    public virtual ICollection<Partidahabilidades> Partidahabilidades { get; set; } = new List<Partidahabilidades>();
}
