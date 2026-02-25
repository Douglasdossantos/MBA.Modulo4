using MBA.Core.Data;

namespace MBA.Aluno.API.Models
{
    public interface IAlunoRepository : IRepository<Aluno>
    {
        Task<IEnumerable<Aluno>> ObterTodosAlunos();
        Task<Aluno> ObterAlunoId(Guid id);
        void AdicionarAluno(Aluno aluno);
    }
}
