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
		var useSqlServer = System.Environment.GetEnvironmentVariable("DATABASE_PROVIDER") switch
		{
			"SqlServer" => true,
			"Sqlite" => false,
			_ => builder.Environment.EnvironmentName != "Development"
		};

		if (useSqlServer)
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));
		else
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlite(connectionString));
	}
}