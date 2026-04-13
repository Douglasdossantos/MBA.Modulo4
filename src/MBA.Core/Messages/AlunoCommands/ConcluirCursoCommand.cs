namespace MBA.Core.Messages.AlunoCommands;

public class ConcluirCursoCommand : CommandRaiz
{
	public Guid AlunoId { get; init; }
	public Guid MatriculaId { get; private set; }

	public ConcluirCursoCommand(Guid matriculaId, Guid alunoId)
	{
		DefinirRaizAgregacao(alunoId);

		MatriculaId = matriculaId;
		AlunoId = alunoId;
	}
}