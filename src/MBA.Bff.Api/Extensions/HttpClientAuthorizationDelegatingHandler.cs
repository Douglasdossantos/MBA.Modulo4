using MBA.WebApi.Core.Usuario;
using System.Net.Http.Headers;

namespace MBA.Bff.Api.Extensions
{
    public class HttpClientAuthorizationDelegatingHandler(IAspNetUser aspNetUser, ILogger<HttpClientAuthorizationDelegatingHandler> logger) : DelegatingHandler
    {
        private readonly IAspNetUser _aspNetUser = aspNetUser;
        private readonly ILogger<HttpClientAuthorizationDelegatingHandler> _logger = logger;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _aspNetUser.ObterHttpContext();
            if (context == null)
                return await base.SendAsync(request, cancellationToken);

            // Prefer existing Authorization header from the incoming request
            if (context.Request.Headers.TryGetValue("Authorization", out var incomingAuth) && !string.IsNullOrWhiteSpace(incomingAuth))
            {
                var header = incomingAuth.ToString().Trim();

                // remove surrounding quotes if present
                if (header.Length >= 2 && ((header.StartsWith("\"") && header.EndsWith("\"")) || (header.StartsWith("'") && header.EndsWith("'"))))
                {
                    header = header.Substring(1, header.Length - 2).Trim();
                }

                // URL decode if someone encoded the header
                try { header = System.Net.WebUtility.UrlDecode(header); } catch { }

                // Try to extract a JWT token pattern (xxx.yyy.zzz)
                var jwtMatch = System.Text.RegularExpressions.Regex.Match(header, "([A-Za-z0-9_-]+\\.[A-Za-z0-9_-]+\\.[A-Za-z0-9_-]+)");
                string token = null;
                if (jwtMatch.Success)
                {
                    token = jwtMatch.Groups[1].Value;
                }
                else if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = header["Bearer ".Length..].Trim();
                }

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    _logger.LogDebug("Forwarding normalized Authorization token to {Url}. Token fragment: {Frag}", request.RequestUri, token?.Substring(0, Math.Min(8, token.Length)));
                }
                else
                {
                    // fallback: set header as-is
                    request.Headers.TryAddWithoutValidation("Authorization", header);
                    _logger.LogDebug("Forwarding incoming raw Authorization header to {Url}", request.RequestUri);
                }

                return await base.SendAsync(request, cancellationToken);
            }

            // Otherwise use token from claims (set by cookie authentication)
            var claimToken = _aspNetUser.ObterUserToken();
            if (!string.IsNullOrWhiteSpace(claimToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", claimToken);
                _logger.LogDebug("Setting Authorization header from claim for {Url}. Token fragment: {Frag}", request.RequestUri, claimToken?.Substring(0, Math.Min(8, claimToken.Length)));
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Outgoing Authorization header to {Url}: {Auth}", request.RequestUri, request.Headers.Authorization?.ToString());
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
