using MBA.Aluno.Domain.Entities;
using MBA.Core.Data;

namespace MBA.Aluno.Domain.Interface;

public interface IAulaAssistidaRepository : IRepository<AulaAssistida>
{
	Task AdicionarAsync(AulaAssistida aulaAssistida);
	Task<bool> CheckAulaJaAssistida(Guid matriculaCursoId, Guid aulaId);
}
