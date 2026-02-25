using MBA.Aluno.API.Models;
using MBA.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace MBA.Aluno.API.Data.Repository
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly AlunoContext _context;

        public AlunoRepository(AlunoContext context)
        {
            _context = context;
        }
        public IUnitOfWork UnitOfWork => _context;

        public async Task<IEnumerable<Models.Aluno>> ObterTodosAlunos()
        {
            return await _context.Alunos.AsNoTracking().ToListAsync();
        }
        public async Task<Models.Aluno> ObterAlunoId(Guid id)
        {
            return await _context.Alunos.FindAsync(id);
        }

        public void AdicionarAluno(Models.Aluno aluno)
        {
            _context.Alunos.Add(aluno);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        
    }
}
