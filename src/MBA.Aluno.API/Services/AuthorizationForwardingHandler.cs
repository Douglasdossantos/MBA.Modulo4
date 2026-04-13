using System.Net.Http.Headers;

namespace MBA.Aluno.API.Services;

/// <summary>
/// DelegatingHandler que repassa o Bearer token do request atual para chamadas
/// HTTP de saída para APIs internas (ex.: Conteúdo API) que exigem autenticação.
/// </summary>
public sealed class AuthorizationForwardingHandler : DelegatingHandler
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public AuthorizationForwardingHandler(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		var context = _httpContextAccessor.HttpContext;

		if (context is not null
			&& context.Request.Headers.TryGetValue("Authorization", out var authHeader)
			&& !string.IsNullOrWhiteSpace(authHeader))
		{
			var raw = authHeader.ToString().Trim();
			var token = raw.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
				? raw["Bearer ".Length..].Trim()
				: raw;

			if (!string.IsNullOrEmpty(token))
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		}

		return await base.SendAsync(request, cancellationToken);
	}
}
