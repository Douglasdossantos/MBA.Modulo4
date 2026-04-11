using MBA.Aluno.Data.Context;
using MBA.Aluno.Domain.Entities;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace MBA.Aluno.Data.Repositories
{
    public class MatriculaRepository(AlunoDbContext context) : IMatriculaRepository
    {
        private readonly AlunoDbContext _context = context;

        public IUnitOfWork UnitOfWork => _context;

        public async Task AdicionarAsync(Matricula matricula)
        {
            await _context.Matriculas.AddAsync(matricula);
        }

        public async Task AtualizarAsync(Matricula matricula)
        {
            _context.Matriculas.Update(matricula);
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<Matricula>> ObterTodosAsync()
        {
            return await _context.Matriculas
                .Include(a => a.Certificado)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Matricula> ObterPorIdAsync(Guid alunoId)
        {
            return await _context.Matriculas
                .Include(a => a.Certificado)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == alunoId);
        }

        public async Task<bool> CheckAlunoJaMatriculado(Guid alunoId, Guid cursoId)
        {
            return await _context.Matriculas
                .AnyAsync(m => m.AlunoId == alunoId && m.CursoId == cursoId);
        }

        public async Task AdicionarAsync(Certificado certificado)
        {
            await _context.Certificados.AddAsync(certificado);
        }
        public async Task<Certificado> ObterCertificadoPorMatriculaAsync(Guid matriculaId)
        {
            return await _context.Certificados
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.MatriculaId == matriculaId);
        }

        public async Task AtualizarStatusAsync(Guid id, Enum status)
        {
            var matricula = await _context.Matriculas.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("Matrícula não encontrada");
            int statusCodigo = Convert.ToInt32(status);
            matricula.AlterarStatusPorCodigo(statusCodigo);

            _context.Matriculas.Update(matricula);
        }


        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }


    }
}
