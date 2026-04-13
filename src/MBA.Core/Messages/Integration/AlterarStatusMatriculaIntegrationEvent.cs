using MBA.Core.SharedDto.Aluno.Enum;

namespace MBA.Core.Messages.Integration;

public class AlterarStatusMatriculaIntegrationEvent : IntegrationEvent
{
	public Guid Id { get; private set; }
	public StatusMatricula Status { get; private set; }

	public AlterarStatusMatriculaIntegrationEvent(Guid id, StatusMatricula status)
	{
		Id = id;
		Status = status;
	}
}