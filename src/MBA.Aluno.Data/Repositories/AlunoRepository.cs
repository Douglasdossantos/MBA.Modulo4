using MBA.Aluno.Data.Context;
using MBA.Aluno.Domain.Entities;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Data;

using Microsoft.EntityFrameworkCore;

namespace MBA.Aluno.Data.Repositories;

public class AlunoRepository : IAlunoRepository
{
	private readonly AlunoDbContext _context;

	public AlunoRepository(AlunoDbContext context)
	{
		_context = context;
	}

	public IUnitOfWork UnitOfWork => _context;

	public async Task AdicionarAsync(Domain.Entities.Aluno aluno)
	{
		await _context.Alunos.AddAsync(aluno);
	}

	public async Task AtualizarAsync(Domain.Entities.Aluno aluno)
	{
		_context.Alunos.Update(aluno);
		await Task.CompletedTask;
	}

	public async Task<bool> ExisteEmailAsync(string email)
	{
		return await _context.Alunos.AnyAsync(a => a.Email == email);
	}

	public async Task<Domain.Entities.Aluno?> ObterPorIdAsync(Guid alunoId)
	{
		return await _context.Alunos
			.AsNoTracking()
			.FirstOrDefaultAsync(a => a.Id == alunoId);
	}

	public async Task<Domain.Entities.Aluno?> ObterComMatriculasAsync(Guid alunoId)
	{
		// Consulta de leitura do aluno com as matrículas carregadas (usada pelo endpoint PorId);
		// mantida separada do ObterPorIdAsync para não alterar o grafo attachado nos fluxos de update.
		return await _context.Alunos
			.AsNoTracking()
			.Include(a => a.Matriculas)
			.FirstOrDefaultAsync(a => a.Id == alunoId);
	}

	public async Task<Domain.Entities.Aluno?> ObterPorEmailAsync(string email)
	{
		return await _context.Alunos
			.AsNoTracking()
			.FirstOrDefaultAsync(a => a.Email == email);
	}

	public async Task DesativarAsync(Domain.Entities.Aluno aluno)
	{
		aluno.Desativar();
		_context.Alunos.Update(aluno);
		await Task.CompletedTask;
	}

	public async Task AtivarAsync(Domain.Entities.Aluno aluno)
	{
		aluno.Ativar();
		_context.Alunos.Update(aluno);
		await Task.CompletedTask;
	}

	public async Task AdicionarAulaAssistidaAsync(AulaAssistida aulaAssistida)
	{
		await _context.AulaAssistidas.AddAsync(aulaAssistida);
		await Task.CompletedTask;
	}


	public async Task<IEnumerable<AulaAssistida>> AulasAssistidasPorMatricula(Guid matriculaId)
	{
		return await _context.AulaAssistidas
			.AsNoTracking()
			.Where(c => c.MatriculaCursoId == matriculaId)
			.ToListAsync();
	}


	public void Dispose()
	{
		_context.Dispose();
		GC.SuppressFinalize(this);
	}
}