namespace MBA.Conteudo.Api.Configuration;

public class SwaggerAuthorizedMiddleware
{
	private readonly RequestDelegate _next;

	public SwaggerAuthorizedMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		if (context.Request.Path.StartsWithSegments("/swagger")
			&& context.User.Identity is not null && !context.User.Identity.IsAuthenticated)
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return;
		}

		await _next.Invoke(context);
	}
}