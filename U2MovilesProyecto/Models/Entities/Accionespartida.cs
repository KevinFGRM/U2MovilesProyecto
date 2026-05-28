namespace U2MovilesProyecto.Models.Entities;

public partial class Accionespartida
{
    public int IdHistorial { get; set; }

    public int IdPartida { get; set; }

    public int Usuario { get; set; }

    public int Habilidad { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Habilidades HabilidadNavigation { get; set; } = null!;

    public virtual Partidas IdPartidaNavigation { get; set; } = null!;

    public virtual Usuarios UsuarioNavigation { get; set; } = null!;
}
