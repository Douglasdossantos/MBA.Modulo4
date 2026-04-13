using MBA.Core.Data;
using MBA.Core.Extensions;
using MBA.Pagamentos.Data.Contexts;
using MBA.Pagamentos.Domain.Entities;
using MBA.Pagamentos.Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace MBA.Pagamentos.Data.Repositories;

public class FaturamentoRepository(FaturamentoDbContext context) : IFaturamentoRepository
{
	public IUnitOfWork UnitOfWork => context;

	public async Task AdicionarAsync(Pagamento pagamento)
	{
		await context.Pagamentos.AddAsync(pagamento);
	}

	public async Task AtualizarAsync(Pagamento pagamento)
	{
		context.Pagamentos.Update(pagamento);

		context.AtualizarEstadoValueObject(null, pagamento.Cartao);

		await Task.CompletedTask;
	}

	public async Task<Pagamento?> ObterPorMatriculaIdAsync(Guid matriculaId)
	{
		return await context.Pagamentos.FirstOrDefaultAsync(p => p.MatriculaId == matriculaId);
	}

	public void Dispose()
	{
		context.Dispose();
		GC.SuppressFinalize(this);
	}
}