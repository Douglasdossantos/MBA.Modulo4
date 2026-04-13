using MBA.Core.DomainObjects;

namespace MBA.Aluno.API.Models;

public class ProgressoAula : Entity
{
	public Guid MatriculaId { get; init; }
	public Guid AulaId { get; init; }

	public bool Concluida { get; init; }
	public DateTime? ConcluidaEm { get; init; }
}