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
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		DbMigrationHelpers.EnsureSeedData(app).Wait();

		return app;
	}
}