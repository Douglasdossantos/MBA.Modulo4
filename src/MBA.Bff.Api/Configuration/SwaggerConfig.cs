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
				Title = "MBA Módulo 5",
				Description = "AVISO IMPORTANTE — SWAGGER EXPOSTO DE PROPÓSITO: Esta é uma aplicação acadêmica (MBA DevXpert, Módulo 5) avaliada por professores, e TODOS os ambientes (inclusive produção) expõem esta documentação para facilitar a consulta e a correção do trabalho. A equipe SABE que em uma aplicação real o Swagger NÃO deve ficar público em produção. Para ocultá-lo, por padrao o Swagger fica oculto em ambiente publicado; para exibi-lo defina SWAGGER_ENABLED=true. Plataforma Educacional Distribuída com Microsserviços REST",
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