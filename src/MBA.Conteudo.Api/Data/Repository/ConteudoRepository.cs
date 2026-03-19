using MBA.Conteudo.Api.Models;
using MBA.Conteudo.Api.Models.Interfaces;
using MBA.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace MBA.Conteudo.Api.Data.Repository
{
    public class ConteudoRepository : IConteudoRepository
    {
        private readonly ConteudoContext _context;

        public ConteudoRepository(ConteudoContext context)
        {
            _context = context;
        }
        public IUnitOfWork UnitOfWork => _context as IUnitOfWork;

        public async Task AdicionarAsync(Curso curso)
        {
            await _context.Cursos.AddAsync(curso);
        }

        public async Task AtualizarAsync(Curso curso)
        {
            _context.Cursos.Update(curso);
            await Task.CompletedTask;
        }

        public async Task DesativarAsync(Curso curso)
        {
            curso.DesativarCurso();
            _context.Cursos.Update(curso);
            await Task.CompletedTask;
        }

        public async Task<Curso?> ObterPorIdAsync(Guid id)
        {
            return await _context.Cursos
                .Include(c => c.ConteudoProgramatico)
                .Include(c => c.Aulas)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Curso>> ObterTodosAsync()
        {
            return await _context.Cursos
                .AsNoTracking()
                .Include(c => c.ConteudoProgramatico)
                .Include(c => c.Aulas)
                .ToListAsync();
        }

        public async Task<IEnumerable<Curso>> ObterAtivosAsync()
        {
            return await _context.Cursos
                .AsNoTracking()
                .Where(c => c.Ativo && (c.ValidoAte == null || c.ValidoAte.Value.Date >= DateTime.Now.Date))
                .Include(c => c.ConteudoProgramatico)
                .Include(c => c.Aulas)
                .ToListAsync();
        }

        public async Task<bool> ExisteCursoComMesmoNomeAsync(string nome)
        {
            return await _context.Cursos
                .AsNoTracking()
                .AnyAsync(c => c.Nome == nome);
        }

        public async Task AdicionarAulaAsync(Aula aula)
        {
            await _context.Aulas.AddAsync(aula);
        }

        public async Task<Aula> ObterAulaPorIdAsync(Guid aulaId)
        {
            return await _context.Aulas
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == aulaId);
        }

        public async Task<IEnumerable<Aula>> ObterTodasAulasAsync()
        {
            return await _context.Aulas
                .AsNoTracking()
                .Where(a => a.Ativo)
                .OrderBy(a => a.OrdemAula)
                .ToListAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
