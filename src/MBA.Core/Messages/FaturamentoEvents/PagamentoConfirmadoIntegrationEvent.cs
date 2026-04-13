using MBA.Core.Messages.Integration;

namespace MBA.Core.Messages.FaturamentoEvents;

public class PagamentoConfirmadoIntegrationEvent : IntegrationEvent
{
    public Guid MatriculaCursoId { get; init; }
    public Guid AlunoId { get; init; }
    public Guid CursoId { get; init; }
    public bool CursoDisponivel { get; init; }

    public PagamentoConfirmadoIntegrationEvent(
        Guid matriculaCursoId,
        Guid alunoId,
        Guid cursoId,
        bool cursoDisponivel = true)
    {
        MatriculaCursoId = matriculaCursoId;
        AlunoId = alunoId;
        CursoId = cursoId;
        CursoDisponivel = cursoDisponivel;
    }
}
