using System.Security.Claims;

namespace MBA.WebApp.MVC.Extensions
{
    public class AspNetUser : IUser
    {
        private readonly IHttpContextAccessor _accessor;

        public AspNetUser(IHttpContextAccessor acessor)
        {
            _accessor = acessor;
        }

        public string Name => _accessor.HttpContext.User.Identity.Name;
        public Guid ObterUserId()
        {
            return EstaAutenticado() ? Guid.Parse(_accessor.HttpContext.User.GetUserId()) : Guid.Empty;
        }
        public string ObterUserEmail()
        {
            return EstaAutenticado() ?_accessor.HttpContext.User.getUserEmail() : string.Empty;
        }
        public string ObterUserToken()
        {
            return EstaAutenticado() ? _accessor.HttpContext.User.getUserToken() : string.Empty;
        }

        public bool EstaAutenticado()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public IEnumerable<Claim> obterClaims()
        {
            return _accessor.HttpContext.User.Claims;
        }
        public bool PossuiRole(string role)
        {
            return _accessor.HttpContext.User.IsInRole(role);
        }

        public HttpContext ObterHttpContext()
        {
            return _accessor.HttpContext;
        }
    }
}
