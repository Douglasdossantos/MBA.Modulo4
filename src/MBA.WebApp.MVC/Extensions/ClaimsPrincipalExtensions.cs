using System.Security.Claims;

namespace MBA.WebApp.MVC.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }
            var claim = principal.FindFirst("sub");
            return claim?.Value;
        }

        public static string getUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null) 
            {
                throw new ArgumentException(nameof(principal));
            }
            var claim = principal.FindFirst("email");
            return claim?.Value;
        }

        public static string getUserToken(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }
            var claim = principal.FindFirst("JWT");
            return claim?.Value;
        }
    }
}
