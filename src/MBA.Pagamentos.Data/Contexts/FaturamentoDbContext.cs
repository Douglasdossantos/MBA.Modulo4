using MBA.Core.Data;
using MBA.Pagamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace MBA.Pagamentos.Data.Contexts;

[ExcludeFromCodeCoverage]
public class FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Pagamento> Pagamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FaturamentoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public async Task<bool> Commit()
    {
        return await base.SaveChangesAsync() > 0;
    }
}