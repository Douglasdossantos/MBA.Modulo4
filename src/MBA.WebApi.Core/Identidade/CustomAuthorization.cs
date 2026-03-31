using Microsoft.AspNetCore.Http;

namespace MBA.WebApi.Core.Identidade
{
    public class CustomAuthorization
    {
        public static bool ValidarClaimsUsuario(HttpContext context, string claimName, string claimValue)
        {
            if (!context.User.Identity.IsAuthenticated) return false;

            // Direct claim check (existing behavior)
            if (context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue)))
                return true;

            // Support permission-style claims (type = "permission") that hold permission codes like "AD", "AT", etc.
            if (context.User.Claims.Any(c => string.Equals(c.Type, "permission", StringComparison.OrdinalIgnoreCase) &&
                                              (string.Equals(c.Value, claimValue, StringComparison.OrdinalIgnoreCase) || c.Value.Contains(claimValue, StringComparison.OrdinalIgnoreCase))))
                return true;

            // Administrators bypass specific permission checks
            if (context.User.IsInRole("Administrador"))
                return true;

            return false;
        }
    }
}