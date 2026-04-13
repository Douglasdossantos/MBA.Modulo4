namespace MBA.Conteudo.Application.ViewModels;
public class CadastroCursoDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime? ValidoAte { get; set; }

    public string Finalidade { get; set; } = string.Empty;
    public string Ementa { get; set; } = string.Empty;
}
