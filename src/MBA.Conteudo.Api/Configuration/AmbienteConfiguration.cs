using MBA.Conteudo.Api.MigrationHelp;

namespace MBA.Conteudo.Api.Configuration;

public static class AmbienteConfiguration
{
	public static WebApplication ExecutarConfiguracaoAmbiente(this WebApplication app)
	{
		// Swagger exposto em TODOS os ambientes (aplicação acadêmica), salvo quando SWAGGER_ENABLED == "false".
		if (app.Configuration["SWAGGER_ENABLED"] != "false")
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		if (app.Environment.IsDevelopment())
		{
			app.UseCors("Dev");
		}
		else
		{
			app.UseCors("Prod");
		}

		// Em Development e Staging o schema é criado e os dados semeados automaticamente.
		if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
		{
			DbMigrationHelper.AutocarregamentoDadosAsync(app).Wait();
		}

		app.UseStaticFiles();
		app.UseHttpsRedirection();

		app.UseHsts();
		app.Use(async (context, next) =>
		{
			context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self'");
			context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
			context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
			await next();
		});

		app.UseRouting();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();

		return app;
	}
}