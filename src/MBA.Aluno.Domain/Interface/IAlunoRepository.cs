using MBA.Aluno.Domain.Entities;
using MBA.Core.Data;

namespace MBA.Aluno.Domain.Interface
{
    public interface IAlunoRepository : IRepository<Entities.Aluno>
    {
        Task<Entities.Aluno> ObterPorIdAsync(Guid alunoId);
        Task<Entities.Aluno> ObterPorEmailAsync(string email);
        Task<bool> ExisteEmailAsync(string email);
        Task AdicionarAsync(Entities.Aluno aluno);
        Task AtualizarAsync(Entities.Aluno aluno);
        Task AtivarAsync(Entities.Aluno aluno);


        Task AdicionarAulaAssistidaAsync(AulaAssistida aulla);

        Task<IEnumerable<AulaAssistida>> AulasAssistidasPorMatricula(Guid matricula);


    }
}
