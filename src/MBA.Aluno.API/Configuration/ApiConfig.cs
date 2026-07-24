namespace MBA.Aluno.API.Configuration;

public static class ApiConfig
{
	public static IApplicationBuilder UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
	{
		// Swagger é habilitado centralmente em Program.cs (gate SWAGGER_ENABLED); não duplicar aqui.
		app.UseHttpsRedirection();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		return app;
	}
}
