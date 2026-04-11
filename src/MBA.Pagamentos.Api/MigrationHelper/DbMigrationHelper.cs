using MBA.Pagamentos.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace MBA.Pagamentos.Api.MigrationHelper;

public static class DbMigrationHelper
{
    public static async Task CarregamentoDadosAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<FaturamentoDbContext>>();
        var faturamentoContext = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();

        await ApplyMigrationsAsync(faturamentoContext, logger);
    }

    private static async Task ApplyMigrationsAsync(
        FaturamentoDbContext context,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
        var appliedMigrations = (await context.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();

        if (appliedMigrations.Count == 0 && pendingMigrations.Count > 0)
        {
            logger.LogWarning(
                "[DbMigrationHelper] Banco de dados novo detectado. Criando schema com {Count} migration(s)...",
                pendingMigrations.Count);
        }

        if (pendingMigrations.Count > 0)
        {
            logger.LogInformation(
                "[DbMigrationHelper] Aplicando {Count} migration(s) pendente(s): {Migrations}",
                pendingMigrations.Count,
                string.Join(", ", pendingMigrations));

            await context.Database.MigrateAsync(cancellationToken);

            logger.LogInformation("[DbMigrationHelper] Migrations aplicadas com sucesso.");
        }
        else
        {
            logger.LogInformation("[DbMigrationHelper] Banco de dados atualizado. Nenhuma migration pendente.");
        }
    }
}
