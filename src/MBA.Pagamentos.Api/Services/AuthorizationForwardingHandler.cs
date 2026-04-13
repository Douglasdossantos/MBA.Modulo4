using System.Net.Http.Headers;

namespace MBA.Pagamentos.Api.Services;

public class AuthorizationForwardingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		var httpContext = httpContextAccessor.HttpContext;
		if (httpContext != null && httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
		{
			var raw = authHeader.ToString();
			if (!string.IsNullOrWhiteSpace(raw) && raw.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
			{
				var token = raw["Bearer ".Length..].Trim();
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
		}

		return base.SendAsync(request, cancellationToken);
	}
}
