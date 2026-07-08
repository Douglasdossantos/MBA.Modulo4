using MBA.Auth.Api.Data;
using MBA.Auth.Api.MigrationHelp;
using MBA.WebApi.Core.Extensions;
using MBA.WebApi.Core.Identidade;

namespace MBA.Auth.Api.Configuration;

public static class ApiConfig
{
	public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddControllers();
		services.AddEndpointsApiExplorer();

		services.AddDefaultHealthChecks()
			.AddDbContextCheck<ApplicationDbContext>("auth-db", tags: [HealthCheckExtensions.ReadyTag]);

		return services;
	}

	public static IApplicationBuilder UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
	{
		// Swagger é configurado exclusivamente por UseSwaggerConfiguration (Program.cs), com a regra SWAGGER_ENABLED.

		// Staging também cria o schema e semeia (o gate fino por ambiente/provider fica no helper).
		if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
			DbMigrationHelper.AutocarregamentoDadosAsync(app).Wait();

		app.UseHttpsRedirection();

		app.UseAuthConfiguration();

		app.MapControllers();
		app.MapDefaultHealthChecks();

		return app;
	}
}