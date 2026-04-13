using MBA.Core.Data;
using MBA.Pagamentos.Domain.Entities;

namespace MBA.Pagamentos.Domain.Interfaces;

public interface IFaturamentoRepository : IRepository<Pagamento>
{
	Task AdicionarAsync(Pagamento pagamento);
	Task AtualizarAsync(Pagamento pagamento);
	Task<Pagamento?> ObterPorMatriculaIdAsync(Guid matriculaId);
}