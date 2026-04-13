
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace MBA.Core.Autentications
{
    public class AppIdentityUser : IAppIdentityUser
    {
        private readonly IHttpContextAccessor _accessor;

        public AppIdentityUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public Guid ObterUsuarioId()
        {
            if (!EstahAutenticado()) return Guid.Empty;

            var claim = _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claim))
                claim = _accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            return claim is null ? Guid.Empty : Guid.Parse(claim);
        }

        public string ObterEmail()
        {
            if (!EstahAutenticado()) return string.Empty;

            var claim = _accessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(claim))
                claim = _accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            return claim is null ? string.Empty : claim;
        }

        // Alinhado com as claims emitidas pela Auth API
        // (AuthController.AdicionaClaimsAdmin/ObterClaimsUsuario):
        //   - role "Administrador"
        //   - claim "Administrador" = "ADM"
        private const string AdministradorRole = "Administrador";
        private const string AdministradorClaimType = "Administrador";
        private const string AdministradorClaimValue = "ADM";
        private const string RoleClaimType = "role";

        public bool EhAdministrador()
        {
            if (!EstahAutenticado()) return false;

            var user = _accessor.HttpContext?.User;
            if (user is null) return false;

            // 1) Role claim emitida como "role" em AuthController.cs:152-155
            if (user.IsInRole(AdministradorRole) ||
                user.HasClaim(c =>
                    (c.Type == RoleClaimType || c.Type == ClaimTypes.Role) &&
                    string.Equals(c.Value, AdministradorRole, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // 2) Claim "Administrador" = "ADM" emitida em AuthController.cs:209-217
            return user.HasClaim(c =>
                string.Equals(c.Type, AdministradorClaimType, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(c.Value, AdministradorClaimValue, StringComparison.OrdinalIgnoreCase));
        }

        public bool EstahAutenticado()
        {
            return _accessor.HttpContext?.User.Identity is { IsAuthenticated: true };
        }
    }
}
