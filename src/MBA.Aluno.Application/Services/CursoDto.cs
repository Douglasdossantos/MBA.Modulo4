namespace MBA.Aluno.Application.Services;

public sealed class CursoDto
{
	public Guid Id { get; set; }
	public bool Ativo { get; set; }
	public string Nome { get; set; } = string.Empty;
	public DateTime Validade { get; set; }
	public string Finalidade { get; set; } = string.Empty;
	public string Ementa { get; set; } = string.Empty;

	/// <summary>
	/// Retorna true quando o curso está ativo e a data de validade ainda não expirou.
	/// </summary>
	public bool EstaDisponivel => Ativo && (Validade == default || Validade >= DateTime.UtcNow.Date);
}
