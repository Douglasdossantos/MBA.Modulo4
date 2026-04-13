using Microsoft.AspNetCore.Http;

using System.Security.Claims;

namespace MBA.WebApi.Core.Usuario;

public class AspNetUser(IHttpContextAccessor accessor) : IAspNetUser
{
	public string Name => accessor.HttpContext?.User.Identity?.Name ?? string.Empty;

	public Guid ObterUserId()
	{
		return EstaAutenticado() ? Guid.Parse(accessor.HttpContext!.User.GetUserId()) : Guid.Empty;
	}

	public string ObterUserEmail()
	{
		return EstaAutenticado() ? accessor.HttpContext!.User.GetUserEmail() : "";
	}

	public string ObterUserToken()
	{
		return EstaAutenticado() ? accessor.HttpContext!.User.GetUserToken() : "";
	}

	public bool EstaAutenticado()
	{
		return accessor.HttpContext?.User.Identity is { IsAuthenticated: true };
	}

	public bool PossuiRole(string role)
	{
		return accessor.HttpContext?.User.IsInRole(role) ?? false;
	}

	public IEnumerable<Claim> ObterClaims()
	{
		return accessor.HttpContext?.User.Claims ?? Enumerable.Empty<Claim>();
	}

	public HttpContext ObterHttpContext()
	{
		return accessor.HttpContext!;
	}
}
