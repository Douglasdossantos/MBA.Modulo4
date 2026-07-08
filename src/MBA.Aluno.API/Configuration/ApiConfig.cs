namespace MBA.Aluno.API.Configuration;

public static class ApiConfig
{
	public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddControllers();
		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		services.AddAuthorization();
		services.AddAuthentication();

		return services;
	}

	public static IApplicationBuilder UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
	{
		// Swagger é habilitado centralmente em Program.cs (gate SWAGGER_ENABLED); não duplicar aqui.
		app.UseHttpsRedirection();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		DbMigrationHelpers.EnsureSeedData(app).Wait();

		return app;
	}
}