using System;
using System.Collections.Generic;

namespace MiniJokeRPGAPI.Models.Entities;

public partial class Fcmtokens
{
    public int IdToken { get; set; }

    public int IdUsuario { get; set; }

    public string Token { get; set; } = null!;

    public DateTime? Fecha { get; set; }

    public virtual Usuarios IdUsuarioNavigation { get; set; } = null!;
}
