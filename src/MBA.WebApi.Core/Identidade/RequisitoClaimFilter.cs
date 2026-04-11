#nullable enable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using System.Security.Claims;

namespace MBA.WebApi.Core.Identidade;

public class RequisitoClaimFilter : IAuthorizationFilter
{
	private readonly Claim _claim;
	private readonly ILogger<RequisitoClaimFilter>? _logger;

	public RequisitoClaimFilter(Claim claim, ILogger<RequisitoClaimFilter>? logger = null)
	{
		_claim = claim;
		_logger = logger;
	}

	public void OnAuthorization(AuthorizationFilterContext context)
	{
		var user = context.HttpContext.User;

		if (user.Identity is not { IsAuthenticated: true })
		{
			_logger?.LogWarning("Authorization failed: user not authenticated. Required claim {ClaimType}:{ClaimValue}",
				_claim.Type, _claim.Value);
			context.Result = new StatusCodeResult(401);
			return;
		}

		// Log current user claims for diagnostics
		try
		{
			var claimsList = user.Claims.Select(c => new { c.Type, c.Value }).ToArray();
			_logger?.LogInformation(
				"Authorizing user {Name}. Required claim {ClaimType}:{ClaimValue}. User claims: {Claims}",
				user.Identity.Name ?? "<no-name>", _claim.Type, _claim.Value, claimsList);
		}
		catch (Exception)
		{
			// Diagnostics logging failed; proceed with authorization check
		}

		var ok = CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Type, _claim.Value);
		if (!ok)
		{
			_logger?.LogWarning("Authorization denied for user {Name}. Required claim {ClaimType}:{ClaimValue}",
				user.Identity.Name ?? "<no-name>", _claim.Type, _claim.Value);
			context.Result = new StatusCodeResult(403);
		}
	}
}