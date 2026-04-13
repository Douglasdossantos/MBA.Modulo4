using MBA.Bff.Api.Extensions;
using MBA.Bff.Api.Handlers;
using MBA.Bff.Api.Services.Implementation;
using MBA.Bff.Api.Services.Interface;
using MBA.Core.Authentications;
using MBA.Core.DomainHadlers;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Usuario;

using MediatR;

using Refit;

namespace MBA.Bff.Api.Configuration;

public static class DependencyInjectionConfig
{
	public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

		var appServices = configuration.GetSection("AppServicesSettings").Get<AppServicesSettings>() ??
						new AppServicesSettings();

		services.AddScoped(sp =>
		{
			var handler = sp.GetRequiredService<HttpClientAuthorizationDelegatingHandler>();
			if (handler.InnerHandler == null)
				handler.InnerHandler = new HttpClientHandler();
			var client = new HttpClient(handler) { BaseAddress = new Uri(appServices.AlunoUrl ?? string.Empty) };
			return RestService.For<IAlunoExternalService>(client);
		});

		services.AddScoped(sp =>
		{
			var handler = sp.GetRequiredService<HttpClientAuthorizationDelegatingHandler>();
			handler.InnerHandler ??= new HttpClientHandler();
			var client = new HttpClient(handler) { BaseAddress = new Uri(appServices.ConteudoUrl ?? string.Empty) };
			return RestService.For<IConteudoExternalServiceService>(client);
		});

		services.AddScoped(sp =>
		{
			var handler = sp.GetRequiredService<HttpClientAuthorizationDelegatingHandler>();
			if (handler.InnerHandler == null)
				handler.InnerHandler = new HttpClientHandler();
			var client = new HttpClient(handler) { BaseAddress = new Uri(appServices.AutenticacaoUrl ?? string.Empty) };
			return RestService.For<IAutenticacaoExternalService>(client);
		});

		services.AddScoped(sp =>
		{
			var handler = sp.GetRequiredService<HttpClientAuthorizationDelegatingHandler>();
			if (handler.InnerHandler == null)
				handler.InnerHandler = new HttpClientHandler();
			var client = new HttpClient(handler) { BaseAddress = new Uri(appServices.FaturamentoUrl ?? string.Empty) };
			return RestService.For<IFaturamentoExternalService>(client);
		});


		services.AddScoped<IConteudoService, ConteudoService>();
		services.AddScoped<IAlunoService, AlunoService>();
		services.AddScoped<IAutenticacaoService, AutenticacaoService>();


		services.AddTransient<HttpClientAuthorizationDelegatingHandler>();
		services.AddHostedService<AlterarStatusMatriculaIntegrationHandler>();

		services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		services.AddScoped<IAspNetUser, AspNetUser>();
		services.AddHttpContextAccessor();
		services.AddScoped<IAppIdentityUser, AppIdentityUser>();

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatorHandler).Assembly));
		services.AddScoped<IMediatorHandler, MediatorHandler>();
		services.AddScoped<INotificationHandler<DomainNotificacaoRaiz>, DomainNotificacaoHandler>();
	}
}