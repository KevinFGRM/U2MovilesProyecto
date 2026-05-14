using System;
using System.Collections.Generic;

namespace U2MovilesProyecto.Models.Entities;

public partial class Partidapersonajes
{
    public int Id { get; set; }

    public int IdPartida { get; set; }

    public int IdUsuario { get; set; }

    public int IdPersonaje { get; set; }

    public int VidaActual { get; set; }

    public int ManaActual { get; set; }

    public virtual Partidas IdPartidaNavigation { get; set; } = null!;

    public virtual Personajes IdPersonajeNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
