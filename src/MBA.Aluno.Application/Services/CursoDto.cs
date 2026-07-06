namespace MBA.Aluno.Application.Services;

public sealed class CursoDto
{
	public Guid Id { get; set; }
	public bool Ativo { get; set; }
	public bool CursoDisponivel { get; set; }
	public string Nome { get; set; } = string.Empty;
	public DateTime Validade { get; set; }
	public string Finalidade { get; set; } = string.Empty;
	public string Ementa { get; set; } = string.Empty;

	/// <summary>
	/// Retorna true quando o curso está disponível para matrícula.
	/// O contrato atual da Conteúdo API expõe <c>cursoDisponivel</c>; Ativo/Validade são
	/// aceitos como fallback de compatibilidade caso o payload não traga <c>cursoDisponivel</c>.
	/// </summary>
	public bool EstaDisponivel => CursoDisponivel || (Ativo && (Validade == default || Validade >= DateTime.UtcNow.Date));
}
