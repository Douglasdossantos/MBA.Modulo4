using MBA.Bff.Api.Extensions;
using MBA.WebApi.Core.Identidade;

namespace MBA.Bff.Api.Configuration;

public static class ApiConfig
{
	public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration,
		IWebHostEnvironment env)
	{
		services.AddControllers();

		services.Configure<AppServicesSettings>(configuration);

		services.AddCors(options =>
		{
			options.AddPolicy("Total", builder =>
			{
				if (env.IsDevelopment())
				{
					// Ambiente local: mantém permissivo para facilitar o desenvolvimento.
					builder
						.AllowAnyOrigin()
						.AllowAnyMethod()
						.AllowAnyHeader();
					return;
				}

				// Fora de Development, restringe às origens configuradas (Cors:AllowedOrigins).
				var origensPermitidas = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
				builder
					.WithOrigins(origensPermitidas)
					.AllowAnyMethod()
					.AllowAnyHeader();
			});
		});
	}

	public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

		app.UseHttpsRedirection();

		app.UseRouting();

		app.UseCors("Total");

		app.UseAuthConfiguration();

		app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
	}
}