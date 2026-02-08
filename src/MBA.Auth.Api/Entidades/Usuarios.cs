using Microsoft.AspNetCore.Identity;

namespace MBA.Auth.Api.Entidades
{
    public class Usuarios : IdentityUser
    {
        public bool Administrador { get; set; }
    }
}
