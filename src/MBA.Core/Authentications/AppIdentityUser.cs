
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

        public bool EhAdministrador()
        {
            if (!EstahAutenticado()) return false;

            var claim = _accessor.HttpContext?.User.FindFirst("nivel")?.Value;

            if (string.IsNullOrEmpty(claim))
                claim = _accessor.HttpContext?.User.FindFirst("nivel")?.Value;

            return claim is null ? false : string.Equals(claim, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        public bool EstahAutenticado()
        {
            return _accessor.HttpContext?.User.Identity is { IsAuthenticated: true };
        }
    }
}
