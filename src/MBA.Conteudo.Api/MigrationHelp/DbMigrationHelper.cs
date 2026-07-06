using MBA.Conteudo.Data;
using MBA.Conteudo.Domain.Entities;
using MBA.Conteudo.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace MBA.Conteudo.Api.MigrationHelp;

public static class DbMigrationHelper
{
	private static ConteudoContext _conteudoContext = null!;

	public static async Task AutocarregamentoDadosAsync(WebApplication serviceScope)
	{
		var services = serviceScope.Services.CreateScope().ServiceProvider;
		await CarregamentoDadosAsync(services);
	}

	public static async Task CarregamentoDadosAsync(IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
		var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

		_conteudoContext = scope.ServiceProvider.GetRequiredService<ConteudoContext>();

		if (env.IsDevelopment())
		{
			// Ensure the folder for the Sqlite database exists (e.g. "Data")
			try
			{
				var dataDir = Path.Combine(env.ContentRootPath, "Data");
				if (!Directory.Exists(dataDir))
					Directory.CreateDirectory(dataDir);
			}
			catch
			{
				// ignore directory creation errors; migration will surface meaningful errors
			}

			await _conteudoContext.Database.MigrateAsync();
			await PopularDatabaseAsync();
		}
	}

	private static async Task PopularDatabaseAsync()
	{
		if (await _conteudoContext.Cursos.AnyAsync()) return;
		await CriarCursoAsync();
	}

	private static async Task CriarCursoAsync()
	{
		var conteudoCurso1 = new ConteudoProgramatico("Aprender a criar sites e sistemas modernos com .NET",
			"Durante o curso, você vai ver como montar uma aplicação completa: backend em .NET e frontend com Angular.");

		var curso1 = new Curso("Curso de Desenvolvimento Full Stack", 3500m, DateTime.Today.AddYears(2),
			conteudoCurso1);
		curso1.AdicionarAulaSeedDeDados("1 - Fundamentos do .NET", 1, 1, "https://curso.com/aula1",
			new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa1"));
		curso1.AdicionarAulaSeedDeDados("2 - Criando APIs REST com ASP.NET Core", 2, 2, "https://curso.com/aula2",
			new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa2"));

		var conteudoCurso2 = new ConteudoProgramatico(
			"Aprender na prática como gerenciar projetos usando métodos ágeis como Scrum e Kanban",
			"Durante o curso, você vai entender como montar, organizar e tocar times ágeis, entregando valor de forma contínua com frameworks ágeis.");

		var curso2 = new Curso(
			"Gestão Ágil de Projetos com Scrum e Kanban",
			2800m,
			DateTime.Today.AddYears(2),
			conteudoCurso2
		);

		curso2.AdicionarAulaSeedDeDados("1 - O que é o Manifesto Ágil e seus princípios", 1, 1,
			"https://curso.com/aula1", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa3"));
		curso2.AdicionarAulaSeedDeDados("2 - Como funciona o Scrum na prática", 2, 2, "https://curso.com/aula2",
			new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa4"));

		var conteudoCurso3 = new ConteudoProgramatico(
			"Preparar você para o mercado de análise de dados usando as ferramentas mais atuais",
			"Durante o curso, você vai aprender desde como modelar dados até criar dashboards e análises de performance com Power BI.");

		var curso3 = new Curso(
			"Análise de Dados com Power BI e SQL Server",
			3200m,
			DateTime.Today.AddYears(2),
			conteudoCurso3
		);

		curso3.AdicionarAulaSeedDeDados("1 - Introdução à Análise de Dados", 1, 1, "https://curso.com/aula1",
			new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa5"));
		curso3.AdicionarAulaSeedDeDados("2 - Fundamentos de SQL Server para análise", 3, 2, "https://curso.com/aula2",
			new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"));

		await _conteudoContext.Cursos.AddAsync(curso1);
		await _conteudoContext.Cursos.AddAsync(curso2);
		await _conteudoContext.Cursos.AddAsync(curso3);
		await _conteudoContext.SaveChangesAsync();
	}
}