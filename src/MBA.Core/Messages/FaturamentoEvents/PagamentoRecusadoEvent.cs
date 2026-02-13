using SaberOnline.Core.Messages;

namespace MBA.Core.Messages.FaturamentoEvents;
public class PagamentoRecusadoEvent : EventoRaiz
{
    public Guid MatriculaCursoId { get; init; }
    public Guid AlunoId { get; init; }
    public Guid CursoId { get; init; }
    public string Mensagem { get; init; } 

    public PagamentoRecusadoEvent(Guid matriculaCursoId, Guid alunoId, Guid cursoId, string mensagem)
    {
        DefinirRaizAgregacao(matriculaCursoId);

        MatriculaCursoId = matriculaCursoId;
        AlunoId = alunoId;
        CursoId = cursoId;
        Mensagem = mensagem;
    }
}