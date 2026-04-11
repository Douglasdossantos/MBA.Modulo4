using MBA.Aluno.API.Models.Enum;
using MBA.Core.DomainObjects;

namespace MBA.Aluno.API.Models;

public class Matricula : Entity
{
	public new Guid Id { get; init; }
	public Guid AlunoId { get; init; }
	public Guid CursoId { get; init; }

	public StatusMatricula Status { get; init; }
	public DateTime CriadaEm { get; init; }
	public DateTime? AtivadaEm { get; init; }
	public DateTime? FinalizadaEm { get; init; }
}