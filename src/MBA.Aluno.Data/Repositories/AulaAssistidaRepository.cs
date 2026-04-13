using MBA.Aluno.Data.Context;
using MBA.Aluno.Domain.Entities;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Data;

using Microsoft.EntityFrameworkCore;

namespace MBA.Aluno.Data.Repositories;

public class AulaAssistidaRepository(AlunoDbContext context) : IAulaAssistidaRepository
{
	public IUnitOfWork UnitOfWork => context;

	public async Task AdicionarAsync(AulaAssistida aulaAssistida)
	{
		await context.AulaAssistidas.AddAsync(aulaAssistida);
	}

	public async Task<bool> CheckAulaJaAssistida(Guid matriculaCursoId, Guid aulaId)
	{
		return await context.AulaAssistidas
			.AsNoTracking()
			.AnyAsync(a => a.MatriculaCursoId == matriculaCursoId && a.AulaId == aulaId);
	}

	public void Dispose()
	{
		context?.Dispose();
		GC.SuppressFinalize(this);
	}
}
