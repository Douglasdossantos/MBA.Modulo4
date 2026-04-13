using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;

namespace MBA.Aluno.Domain.Entities;

public class AulaAssistida : Entity, IAggregateRoot
{
	public Guid MatriculaCursoId { get; private set; }
	public Guid AulaId { get; set; }
	public DateTime DataTermino { get; set; }

	public AulaAssistida() { }

	public AulaAssistida(Guid matriculaCursoId, Guid aulaId, DateTime dataTermino)
	{
		MatriculaCursoId = matriculaCursoId;
		AulaId = aulaId;
		DataTermino = dataTermino;

		ValidarAulaAssistida();
	}

	public void AlterarAulaId(Guid aulaId)
	{
		ValidarAulaAssistida(aulaId: aulaId);
		AulaId = aulaId;
	}

	public void AlterarDataTermino(DateTime dataTermino)
	{
		ValidarAulaAssistida(dataTermino: dataTermino);
		DataTermino = dataTermino;
	}

	public void ValidarAulaAssistida(Guid? matriculaCursoId = null, Guid? aulaId = null, DateTime? dataTermino = null)
	{
		if (matriculaCursoId != null && matriculaCursoId != Guid.Empty)
			MatriculaCursoId = matriculaCursoId.Value;

		if (aulaId != null && aulaId != Guid.Empty)
			AulaId = aulaId.Value;

		if (dataTermino.HasValue)
			DataTermino = dataTermino.Value;

		Validacoes.ValidarSeVazio(MatriculaCursoId, "O ID da matrícula do curso não pode estar vazio.");
		Validacoes.ValidarSeVazio(AulaId, "O ID da aula não pode estar vazio.");
		Validacoes.ValidarData(DataTermino, "A data da matrícula é inválida.");
	}


	public override string ToString()
	{
		return $"mtricula {MatriculaCursoId}, aula {AulaId}, realizada na data {DataTermino}";
	}
}