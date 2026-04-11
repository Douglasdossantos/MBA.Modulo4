using MBA.WebApp.MVC.Extensions;
using MBA.WebApp.MVC.Services;
using Microsoft.Extensions.Options;

namespace MBA.WebApp.MVC.Configuration;

public static class DependencyInjectionConfig
{
	public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

		services.AddHttpClient<IAutenticacaoService, AutenticacaoService>((sp, client) =>
		{
			var settings = sp.GetRequiredService<IOptions<AppSettings>>().Value;
			client.BaseAddress = new Uri(settings.AutenticacaoUrl);
		});

		services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		services.AddScoped<IUser, AspNetUser>();
	}
}