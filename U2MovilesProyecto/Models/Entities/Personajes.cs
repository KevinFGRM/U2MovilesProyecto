using System;
using System.Collections.Generic;

namespace U2MovilesProyecto.Models.Entities;

public partial class Personajes
{
    public int IdPersonaje { get; set; }

    public string Nombre { get; set; } = null!;

    public int VidaBase { get; set; }

    public int ManaBase { get; set; }

    public int AtaqueBase { get; set; }

    public int DefensaBase { get; set; }

    public virtual ICollection<Habilidades> Habilidades { get; set; } = new List<Habilidades>();

    public virtual ICollection<Partidapersonajes> Partidapersonajes { get; set; } = new List<Partidapersonajes>();
}
