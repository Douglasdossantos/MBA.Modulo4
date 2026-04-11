namespace MBA.Conteudo.Application.ViewModels;
public class AtualizacaoCursoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime? ValidoAte { get; set; }
    public bool Ativo { get; set; }
    public string Finalidade { get; set; } = string.Empty;
    public string Ementa { get; set; } = string.Empty;
}
