namespace MBA.Aluno.Application.Services;

public interface IConteudoService
{
	/// <summary>
	/// Consulta a Conteúdo API para obter os dados de um curso pelo identificador.
	/// Retorna null quando o curso não for encontrado ou não puder ser consultado.
	/// </summary>
	Task<CursoDto?> ObterCursoAsync(Guid cursoId, CancellationToken cancellationToken);
}
