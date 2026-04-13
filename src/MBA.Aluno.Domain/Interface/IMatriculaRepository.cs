using MBA.Aluno.Domain.Entities;
using MBA.Core.Data;

namespace MBA.Aluno.Domain.Interface;

public interface IMatriculaRepository : IRepository<Matricula>
{
	Task AdicionarAsync(Matricula matricula);
	Task AtualizarAsync(Matricula matricula);
	Task<Matricula?> ObterPorIdAsync(Guid id);
	Task<IEnumerable<Matricula>> ObterTodosAsync();
	Task AtualizarStatusAsync(Guid id, Enum status);
	Task AdicionarAsync(Certificado certificado);
	Task<Certificado?> ObterCertificadoPorMatriculaAsync(Guid matriculaId);

	Task<bool> CheckAlunoJaMatriculado(Guid alunoId, Guid cursoId);
}