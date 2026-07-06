using Microsoft.OpenApi.Models;

namespace MBA.Bff.Api.Configuration;

public static class SwaggerConfig
{
	public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
	{
		services.AddEndpointsApiExplorer();

		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "MBA Módulo 4",
				Description = "Plataforma Educacional Distribuída com Microsserviços REST",
				Contact = new OpenApiContact { Name = "Suporte", Email = "contato@desenvolvedor.io" },
				License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
			});
		});

		return services;
	}

	public static WebApplication UseSwaggerConfiguration(this WebApplication app)
	{
		app.UseSwagger();
		app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });

		return app;
	}
}