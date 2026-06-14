using MBA.Aluno.Data.Context;
using MBA.Conteudo.Data;
using MBA.Conteudo.Data.Repository;
using MBA.Conteudo.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MBA.Aluno.API.Configuration;

public static class DataBaseSelectorExtension
{
	public static void AddDatabaseSelector(this WebApplicationBuilder builder)
	{
		var provider = builder.Environment.EnvironmentName;

		var dbName = builder.Configuration.GetConnectionString("ConnectionStringAluno");

		var dbPath = Path.Combine(
			Directory.GetCurrentDirectory(),
			"..",
			"MBA.Aluno.Data",
			dbName ?? string.Empty);

		var sqliteConnection = $"Data Source={dbPath}";

		var conteudoDbName = builder.Configuration.GetConnectionString("ConnectionStringConteudo");
		var conteudoDbPath = Path.Combine(
			Directory.GetCurrentDirectory(),
			"..",
			"MBA.Conteudo.Data",
			conteudoDbName ?? string.Empty);
		var conteudoSqliteConnection = $"Data Source={conteudoDbPath}";

		// T-14: provider explícito via env var (DATABASE_PROVIDER=SqlServer|Sqlite) vence; sem ela,
		// mantém o fallback por ambiente (não-Development => SQL Server).
		var useSqlServer = System.Environment.GetEnvironmentVariable("DATABASE_PROVIDER") switch
		{
			"SqlServer" => true,
			"Sqlite" => false,
			_ => provider != "Development"
		};

		if (useSqlServer)
		{
			var connectionString = builder.Configuration.GetConnectionString("ConnectionStringAluno");
			var conteudoConnectionString = builder.Configuration.GetConnectionString("ConnectionStringConteudo");

			builder.Services.AddDbContext<AlunoDbContext>(options =>
				options.UseSqlServer(connectionString));
			builder.Services.AddDbContext<ConteudoContext>(options =>
				options.UseSqlServer(conteudoConnectionString));
		}
		else
		{
			builder.Services.AddDbContext<AlunoDbContext>(options =>
				options.UseSqlite(sqliteConnection));
			builder.Services.AddDbContext<ConteudoContext>(options =>
				options.UseSqlite(conteudoSqliteConnection));
		}

		builder.Services.AddScoped<IConteudoRepository, ConteudoRepository>();
	}
}