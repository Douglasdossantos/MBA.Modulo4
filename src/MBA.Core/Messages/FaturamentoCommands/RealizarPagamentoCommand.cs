using MBA.Core.Messages;
using MBA.Core.SharedDto;


namespace MBA.Messages.FaturamentoCommands;
public class RealizarPagamentoCommand : CommandRaiz
{
    public Guid MatriculaCursoId { get; init; }

    public Guid CursoId { get; set; }
    public Guid AlunoId { get; set; }
    public bool PagamentoPodeSerRealizado { get; set; }
    public string NomeCurso { get; set; }
    public decimal Valor { get; set; }
    public DateTime DataMatricula { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string EstadoMatricula { get; set; }

    public string NumeroCartao { get; init; }
    public string NomeTitularCartao { get; init; }
    public string ValidadeCartao { get; init; }
    public string CvvCartao { get; init; }

    public RealizarPagamentoCommand(Guid matriculaCursoId, Guid cursoId, Guid alunoId, bool pagamentoPodeSerRealizado, string nomeCurso, decimal valor, DateTime dataMatricula, DateTime? dataConclusao, string estadoMatricula, string numeroCartao, string nomeTitularCartao, string validadeCartao, string cvvCartao)
    {
        MatriculaCursoId = matriculaCursoId;
        CursoId = cursoId;
        AlunoId = alunoId;
        PagamentoPodeSerRealizado = pagamentoPodeSerRealizado;
        NomeCurso = nomeCurso;
        Valor = valor;
        DataMatricula = dataMatricula;
        DataConclusao = dataConclusao;
        EstadoMatricula = estadoMatricula;
        NumeroCartao = numeroCartao;
        NomeTitularCartao = nomeTitularCartao;
        ValidadeCartao = validadeCartao;
        CvvCartao = cvvCartao;
    }
}
