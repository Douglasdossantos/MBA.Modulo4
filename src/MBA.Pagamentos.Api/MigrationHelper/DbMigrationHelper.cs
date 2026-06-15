using MBA.Pagamentos.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace MBA.Pagamentos.Api.MigrationHelper;

public static class DbMigrationHelper
{
	private static FaturamentoDbContext _faturamentoContext = null!;

	public static async Task CarregamentoDadosAsync(this IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

		_faturamentoContext = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();

		await _faturamentoContext.Database.MigrateAsync();
	}
}