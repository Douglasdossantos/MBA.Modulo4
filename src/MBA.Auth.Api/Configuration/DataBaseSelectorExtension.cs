using MBA.Auth.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace MBA.Auth.Api.Configuration;

public static class DataBaseSelectorExtension
{
	public static void AddDatabaseSelector(this WebApplicationBuilder builder)
	{
		var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

		// T-14: provider explícito via env var (DATABASE_PROVIDER=SqlServer|Sqlite) vence; sem ela,
		// mantém o fallback por ambiente (não-Development => SQL Server).
		var useSqlServer = Environment.GetEnvironmentVariable("DATABASE_PROVIDER") switch
		{
			"SqlServer" => true,
			"Sqlite" => false,
			_ => builder.Environment.EnvironmentName != "Development"
		};

		if (useSqlServer)
		{
#if DEBUG
			// Outros devs do time não têm acesso ao SQL Server publicado. Em Debug local o SQL Server
			// vira LocalDB automaticamente. As imagens Docker são publish -c Release, então este guard
			// não existe no binário publicado (diretiva de compilação, não checagem de runtime).
			connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=mba-auth-localdb;Trusted_Connection=True;MultipleActiveResultSets=true";
#endif
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));
		}
		else
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlite(connectionString));
	}
}