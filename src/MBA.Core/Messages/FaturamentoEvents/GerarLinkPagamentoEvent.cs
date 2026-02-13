namespace MBA.Core.Messages.FaturamentoEvents;
public class GerarLinkPagamentoEvent : EventoRaiz
{
    public Guid MatriculaCursoId { get; init; }
    public Guid AlunoId { get; init; }
    public Guid CursoId { get; init; }
    public decimal Valor { get; init; }

    public GerarLinkPagamentoEvent(Guid matriculaCursoId, Guid alunoId, Guid cursoId, decimal valor)
    {
        DefinirRaizAgregacao(matriculaCursoId);

        MatriculaCursoId = matriculaCursoId;
        AlunoId = alunoId;
        CursoId = cursoId;
        Valor = valor;
    }
}
