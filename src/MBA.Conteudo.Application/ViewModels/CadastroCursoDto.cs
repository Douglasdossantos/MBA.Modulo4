namespace MBA.Conteudo.Application.ViewModels;
public class CadastroCursoDto
{
    public string Nome { get; set; }
    public decimal Valor { get; set; }
    public DateTime? ValidoAte { get; set; }

    public string Finalidade { get; set; }
    public string Ementa { get; set; }
}
