using MBA.Conteudo.Domain.Entities;
using MBA.Core.Data;

namespace MBA.Conteudo.Domain.Interfaces;

public interface IConteudoRepository : IRepository<Curso>
{
	Task AdicionarAsync(Curso curso);
	Task AtualizarAsync(Curso curso);
	Task DesativarAsync(Curso curso);
	Task<Curso?> ObterPorIdAsync(Guid id);
	Task<IEnumerable<Curso>> ObterTodosAsync();
	Task<IEnumerable<Curso>> ObterAtivosAsync();
	Task<bool> ExisteCursoComMesmoNomeAsync(string nome);

	Task AdicionarAulaAsync(Aula aula);
}