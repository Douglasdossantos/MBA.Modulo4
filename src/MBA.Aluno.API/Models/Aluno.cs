using MBA.Core.DomainObjects;

namespace MBA.Aluno.API.Models;

public class Aluno : Entity
{
	public DateTime CriadoEm { get; init; }

	public ICollection<Matricula> Matriculas { get; init; } = [];
}