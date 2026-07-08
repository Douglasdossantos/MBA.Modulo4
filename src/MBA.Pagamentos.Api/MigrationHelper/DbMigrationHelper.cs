using MBA.Pagamentos.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MBA.Pagamentos.Api.MigrationHelper;

public static class DbMigrationHelper
{
	private static FaturamentoDbContext _faturamentoContext = null!;

	public static async Task CarregamentoDadosAsync(this IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
		var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

		_faturamentoContext = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();

		if (env.IsDevelopment() || env.IsStaging())
		{
			// As migrations existentes são específicas de SQLite. No SQL Server o schema é criado
			// direto do modelo (EnsureCreated); no SQLite mantém Migrate.
			if (_faturamentoContext.Database.IsSqlServer())
				await _faturamentoContext.Database.EnsureCreatedAsync();
			else
				await _faturamentoContext.Database.MigrateAsync();
		}
	}
}