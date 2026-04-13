using MBA.WebApi.Core.Usuario;

using System.Net.Http.Headers;

namespace MBA.Bff.Api.Extensions;

public class HttpClientAuthorizationDelegatingHandler(
	IAspNetUser aspNetUser,
	ILogger<HttpClientAuthorizationDelegatingHandler> logger) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		var context = aspNetUser.ObterHttpContext();
		if (context == null)
			return await base.SendAsync(request, cancellationToken);

		if (context.Request.Headers.TryGetValue("Authorization", out var incomingAuth) &&
			!string.IsNullOrWhiteSpace(incomingAuth))
		{
			var header = incomingAuth.ToString().Trim();

			if (header.Length >= 2
				&& ((header.StartsWith("\"") && header.EndsWith("\""))
					|| (header.StartsWith("'") && header.EndsWith("'"))))
				header = header.Substring(1, header.Length - 2).Trim();

			try
			{
				header = System.Net.WebUtility.UrlDecode(header);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error decoding Authorization header: {Message}", ex.Message);
			}

			var jwtMatch =
				System.Text.RegularExpressions.Regex.Match(header,
					"([A-Za-z0-9_-]+\\.[A-Za-z0-9_-]+\\.[A-Za-z0-9_-]+)");
			string token = null;
			if (jwtMatch.Success)
				token = jwtMatch.Groups[1].Value;
			else if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
				token = header["Bearer ".Length..].Trim();

			if (!string.IsNullOrEmpty(token))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
				logger.LogDebug("Forwarding normalized Authorization token to {Url}. Token fragment: {Frag}",
					request.RequestUri, token[..Math.Min(8, token.Length)]);
			}
			else
			{
				request.Headers.TryAddWithoutValidation("Authorization", header);
				logger.LogDebug("Forwarding incoming raw Authorization header to {Url}", request.RequestUri);
			}

			return await base.SendAsync(request, cancellationToken);
		}

		var claimToken = aspNetUser.ObterUserToken();
		if (!string.IsNullOrWhiteSpace(claimToken))
		{
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", claimToken);
			logger.LogDebug("Setting Authorization header from claim for {Url}. Token fragment: {Frag}",
				request.RequestUri, claimToken[..Math.Min(8, claimToken.Length)]);
			if (logger.IsEnabled(LogLevel.Debug))
				logger.LogDebug("Outgoing Authorization header to {Url}: {Auth}", request.RequestUri,
					request.Headers.Authorization?.ToString());
		}

		return await base.SendAsync(request, cancellationToken);
	}
}