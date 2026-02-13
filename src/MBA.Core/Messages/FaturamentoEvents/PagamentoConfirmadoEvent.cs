

namespace MBA.Core.Messages.FaturamentoEvents;
public class PagamentoConfirmadoEvent : EventoRaiz
{
    public Guid MatriculaCursoId { get; init; }
    public Guid AlunoId { get; init; }
    public Guid CursoId { get; init; }
    public bool CursoDisponivel { get; set; }

    public PagamentoConfirmadoEvent(Guid matriculaCursoId, Guid alunoId, Guid cursoId, bool cursoDisponivel)
    {
        DefinirRaizAgregacao(matriculaCursoId);

        MatriculaCursoId = matriculaCursoId;
        AlunoId = alunoId;
        CursoId = cursoId;
        CursoDisponivel = cursoDisponivel;
    }
}