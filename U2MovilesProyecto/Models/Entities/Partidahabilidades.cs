using System;
using System.Collections.Generic;

namespace MiniJokeRPGAPI.Models.Entities;

public partial class Partidahabilidades
{
    public int Id { get; set; }

    public int IdPartida { get; set; }

    public int IdUsuario { get; set; }

    public int IdHabilidad { get; set; }

    public virtual Habilidades IdHabilidadNavigation { get; set; } = null!;

    public virtual Partidas IdPartidaNavigation { get; set; } = null!;

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
