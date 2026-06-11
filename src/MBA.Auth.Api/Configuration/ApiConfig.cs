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
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });

			DbMigrationHelper.AutocarregamentoDadosAsync(app).Wait();
		}

		app.UseHttpsRedirection();

		app.UseAuthConfiguration();

		app.MapControllers();
		app.MapDefaultHealthChecks();

		return app;
	}
}