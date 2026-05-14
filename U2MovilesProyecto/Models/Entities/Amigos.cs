using System;
using System.Collections.Generic;

namespace U2MovilesProyecto.Models.Entities;

public partial class Amigos
{
    public int IdRelacion { get; set; }

    public int Usuario1 { get; set; }

    public int Usuario2 { get; set; }

    public string? Estado { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Usuarios Usuario1Navigation { get; set; } = null!;

    public virtual Usuarios Usuario2Navigation { get; set; } = null!;
}
