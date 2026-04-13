using MBA.Bff.Api.Extensions;
using MBA.Bff.Api.Handlers;
using MBA.Bff.Api.Services.Implementation;
using MBA.Bff.Api.Services.Interface;
using MBA.Core.Authentications;
using MBA.Core.DomainHadlers;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Extensions;
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

		services.AddRefitClient<IAlunoExternalService>()
			.ConfigureHttpClient(client =>
			{
				client.BaseAddress = new Uri(appServices.AlunoUrl ?? string.Empty);
				client.Timeout = TimeSpan.FromSeconds(30);
			})
			.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
			.AddPolicyHandler(PollyExtensions.EsperarTentar())
			.AddPolicyHandler(PollyExtensions.CircuitBreaker());

		services.AddRefitClient<IConteudoExternalServiceService>()
			.ConfigureHttpClient(client =>
			{
				client.BaseAddress = new Uri(appServices.ConteudoUrl ?? string.Empty);
				client.Timeout = TimeSpan.FromSeconds(30);
			})
			.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
			.AddPolicyHandler(PollyExtensions.EsperarTentar())
			.AddPolicyHandler(PollyExtensions.CircuitBreaker());

		services.AddRefitClient<IAutenticacaoExternalService>()
			.ConfigureHttpClient(client =>
			{
				client.BaseAddress = new Uri(appServices.AutenticacaoUrl ?? string.Empty);
				client.Timeout = TimeSpan.FromSeconds(30);
			})
			.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
			.AddPolicyHandler(PollyExtensions.EsperarTentar())
			.AddPolicyHandler(PollyExtensions.CircuitBreaker());

		services.AddRefitClient<IFaturamentoExternalService>()
			.ConfigureHttpClient(client =>
			{
				client.BaseAddress = new Uri(appServices.FaturamentoUrl ?? string.Empty);
				client.Timeout = TimeSpan.FromSeconds(30);
			})
			.AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
			.AddPolicyHandler(PollyExtensions.EsperarTentar())
			.AddPolicyHandler(PollyExtensions.CircuitBreaker());

		services.AddScoped<IConteudoService, ConteudoService>();
		services.AddScoped<IAlunoService, AlunoService>();
		services.AddScoped<IAutenticacaoService, AutenticacaoService>();

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
