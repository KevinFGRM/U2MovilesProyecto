using System;
using System.Collections.Generic;

namespace MiniJokeRPGAPI.Models.Entities;

public partial class Efectospartida
{
    public int IdEfecto { get; set; }

    public int IdPartida { get; set; }

    public int IdUsuario { get; set; }

    public string TipoEfecto { get; set; } = null!;

    public int Valor { get; set; }

    public int TurnosRestantes { get; set; }
}
