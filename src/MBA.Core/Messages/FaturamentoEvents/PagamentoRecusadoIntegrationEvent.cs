using MBA.Core.Messages.Integration;

namespace MBA.Core.Messages.FaturamentoEvents;

public class PagamentoRecusadoIntegrationEvent : IntegrationEvent
{
    public Guid MatriculaCursoId { get; init; }
    public Guid AlunoId { get; init; }
    public Guid CursoId { get; init; }
    public string MotivoRecusa { get; init; }

    public PagamentoRecusadoIntegrationEvent(
        Guid matriculaCursoId,
        Guid alunoId,
        Guid cursoId,
        string motivoRecusa)
    {
        MatriculaCursoId = matriculaCursoId;
        AlunoId = alunoId;
        CursoId = cursoId;
        MotivoRecusa = motivoRecusa;
    }
}
