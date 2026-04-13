namespace MBA.Aluno.Application.Services;

public interface IConteudoService
{
	/// <summary>
	/// Consulta a Conteúdo API para obter os dados de um curso pelo identificador.
	/// Retorna null quando o curso não for encontrado ou não puder ser consultado.
	/// </summary>
	Task<CursoDto?> ObterCursoAsync(Guid cursoId, CancellationToken cancellationToken);

	/// <summary>
	/// Retorna o total de aulas cadastradas em um curso. Fail-safe: retorna 0
	/// quando o curso não existe ou a Conteúdo API está indisponível.
	/// </summary>
	Task<int> ObterTotalAulasAsync(Guid cursoId, CancellationToken cancellationToken);
}
