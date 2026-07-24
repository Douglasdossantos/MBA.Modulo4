using Microsoft.OpenApi.Models;

namespace MBA.Aluno.API.Configuration;

public static class SwaggerConfig
{
	public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
	{
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "DevStore Enterprise Aluno API",
				Description =
					"AVISO IMPORTANTE — SWAGGER EXPOSTO DE PROPÓSITO: Esta é uma aplicação acadêmica " +
					"(MBA DevXpert, Módulo 5) avaliada por professores, e TODOS os ambientes (inclusive " +
					"produção) expõem esta documentação para facilitar a consulta e a correção do trabalho. " +
					"A equipe SABE que em uma aplicação real o Swagger NÃO deve ficar público em produção. " +
					"Para ocultá-lo, por padrao o Swagger fica oculto em ambiente publicado; para exibi-lo defina SWAGGER_ENABLED=true." +
					"\n\n" +
					"essa API pertence ao Modulo 5 do MBA DEVXPERT FULL STACK .NET, Plataforma Educacional Distribuída com Microsserviços REST",
				Contact = new OpenApiContact { Name = "Douglas dos santos", Email = "grupo@email.com" },
				License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/Licenses/MIT") }
			});

			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "Insira o token JWT desta maneira: Bearer {seu token}",
				Name = "Authorization",
				Scheme = "Bearer",
				BearerFormat = "JWT",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					new string[] { }
				}
			});
		});
		return services;
	}

	public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
	{
		app.UseSwagger();
		app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });

		return app;
	}
}