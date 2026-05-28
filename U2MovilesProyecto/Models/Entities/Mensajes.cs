using System;
using System.Collections.Generic;

namespace MiniJokeRPGAPI.Models.Entities;

public partial class Mensajes
{
    public int IdMensaje { get; set; }

    public int Emisor { get; set; }

    public int Receptor { get; set; }

    public string Mensaje { get; set; } = null!;

    public DateTime? Fecha { get; set; }

    public virtual Usuarios EmisorNavigation { get; set; } = null!;

    public virtual Usuarios ReceptorNavigation { get; set; } = null!;
}
