using MBA.Core.SharedDto.Aluno;

namespace MBA.Aluno.Application.Interfaces;

public interface IAlunoQuery
{
	public Task<MatriculaDto?> EvolucaoCursoPorMatriculaAsync(Guid matriculaId);
	public Task<MatriculaStatusDto?> ObterStatusMatriculaAsync(Guid matriculaId, CancellationToken cancellationToken);
}