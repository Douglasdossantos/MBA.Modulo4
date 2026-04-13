using MBA.Core.SharedDto.Aluno;

namespace MBA.Pagamentos.Application.Services;

public interface IAlunoService
{
	Task<MatriculaStatusDto?> ObterStatusMatriculaAsync(Guid matriculaId, CancellationToken cancellationToken);
}
