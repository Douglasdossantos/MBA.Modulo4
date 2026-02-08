using MBA.WebApi.Core.Usuario;
using System.Net.Http.Headers;

namespace MBA.Bff.Api.Extensions
{
    public class HttpClientAuthorizationDelegatingHandler(IAspNetUser aspNetUser) : DelegatingHandler
    {
        private readonly IAspNetUser _aspNetUser = aspNetUser;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authorizationHeader = _aspNetUser.ObterHttpContext().Request.Headers.Authorization;

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", [authorizationHeader!]);
            }

            var token = _aspNetUser.ObterUserToken();

            if (token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
