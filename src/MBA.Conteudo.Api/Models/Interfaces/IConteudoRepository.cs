using MBA.Core.Data;

namespace MBA.Conteudo.Api.Models.Interfaces
{
    public interface IConteudoRepository : IRepository<Curso>
    {
        Task AdicionarAsync(Curso curso);
        Task AtualizarAsync(Curso curso);
        Task DesativarAsync(Curso curso);
        Task<Curso> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Curso>> ObterTodosAsync();
        Task<IEnumerable<Curso>> ObterAtivosAsync();
        Task<bool> ExisteCursoComMesmoNomeAsync(string nome);

        Task AdicionarAulaAsync(Aula aula);
        Task<Aula> ObterAulaPorIdAsync(Guid aulaId);
        Task<IEnumerable<Aula>> ObterTodasAulasAsync();
    }
}
