namespace MBA.Bff.Api.Models.Conteudo;

public class CursoViewModel
{
	public string Nome { get; set; } = string.Empty;

	public decimal Valor { get; set; }

	public DateTime? ValidoAte { get; set; }

	public ConteudoProgramaticoViewModel ConteudoProgramatico { get; set; } = new();
}