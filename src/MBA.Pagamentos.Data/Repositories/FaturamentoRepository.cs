using MBA.Core.Data;
using MBA.Core.Extensions;
using MBA.Pagamentos.Data.Contexts;
using MBA.Pagamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MBA.Pagamentos.Data.Repositories;
public class FaturamentoRepository(FaturamentoDbContext context) : IFaturamentoRepository
{
    private readonly FaturamentoDbContext _context = context;
    public IUnitOfWork UnitOfWork => _context;

    public async Task AdicionarAsync(Pagamento pagamento)
    {
        await _context.Pagamentos.AddAsync(pagamento);
    }

    public async Task AtualizarAsync(Pagamento pagamento)
    {
        _context.Pagamentos.Update(pagamento);

        if (pagamento.Cartao != null)
        {
            _context.AtualizarEstadoValueObject(null, pagamento.Cartao);
        }

        await Task.CompletedTask;
    }

    public async Task<Pagamento> ObterPorMatriculaIdAsync(Guid matriculaId)
    {
        return await _context.Pagamentos.FirstOrDefaultAsync(p => p.MatriculaId == matriculaId);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
  
}