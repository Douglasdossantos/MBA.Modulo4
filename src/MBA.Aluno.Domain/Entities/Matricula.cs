using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;
using MBA.Core.SharedDto.Aluno;
using MBA.Core.SharedDto.Aluno.Enum;

namespace MBA.Aluno.Domain.Entities;

public class Matricula : Entity, IAggregateRoot
{
	public Matricula(Guid cursoId, Guid alunoId, DateTime dataMatricula, StatusMatricula status)
	{
		CursoId = cursoId;
		AlunoId = alunoId;
		DataMatricula = dataMatricula;
		Status = status;

		ValidarMatricula();
	}

	public Guid CursoId { get; private set; }
	public Guid AlunoId { get; private set; }
	public DateTime DataMatricula { get; private set; }
	public DateTime? DataCursoConcluido { get; private set; }
	public StatusMatricula Status { get; private set; }
	public Aluno? Aluno { get; set; }
	public Certificado? Certificado { get; init; }


	public void CriarData()
	{
		DataMatricula = DateTime.Now;
	}

	public void CriarDataConcluido()
	{
		DataCursoConcluido = DateTime.Now;
	}

	public void StatusCancelada()
	{
		Status = StatusMatricula.Cancelada;
	}

	public void StatusPendentePagamento()
	{
		Status = StatusMatricula.PendentePagamento;
	}

	public void StatusPagamentoRealizado()
	{
		Status = StatusMatricula.PagamentoRealizado;
	}

	public void StatusConcluido()
	{
		Status = StatusMatricula.Concluido;
	}

	public void AlterarCursoId(Guid cursoId)
	{
		ValidarMatricula(cursoId);
		CursoId = cursoId;
	}

	public void AlterarAlunoId(Guid alunoId)
	{
		ValidarMatricula(alunoId: alunoId);
		AlunoId = alunoId;
	}

	private void ValidarMatricula(Guid? cursoId = null, Guid? alunoId = null, DateTime? dataMatricula = null)
	{
		if (cursoId != null && cursoId != Guid.Empty)
			CursoId = cursoId.Value;

		if (alunoId != null && alunoId != Guid.Empty)
			AlunoId = alunoId.Value;

		if (dataMatricula != null && dataMatricula != DateTime.MinValue)
			DataMatricula = dataMatricula.Value;

		Validacoes.ValidarSeVazio(CursoId, "O ID do curso não pode ser vazio.");
		Validacoes.ValidarSeVazio(AlunoId, "O ID do aluno não pode ser vazio.");
		Validacoes.ValidarData(DataMatricula, "A data da matrícula é inválida.");
	}

	public void AlterarStatusPorCodigo(int codigo)
	{
		switch (codigo)
		{
			case 1:
				StatusPendentePagamento();
				break;

			case 2:
				StatusPagamentoRealizado();
				break;

			case 3:
				StatusConcluido();
				break;

			case 4:
				StatusCancelada();
				break;


			default:
				throw new ArgumentException("Código de status inválido");
		}
	}

	public static implicit operator MatriculaDto(Matricula? matricula)
	{
		if (matricula is null) return new MatriculaDto();

		return new MatriculaDto
		{
			Id = matricula.Id,
			CursoId = matricula.CursoId,
			AlunoId = matricula.AlunoId,
			DataMatricula = matricula.DataMatricula,
			DataCursoConcluido = matricula.DataCursoConcluido ?? DateTime.MinValue,
			Status = matricula.Status,
			Certificado = matricula.Certificado
		};
	}

	public override string ToString()
	{
		return
			$"matrícula com aluno {AlunoId}, curso {CursoId}, realizada na data {DataMatricula}, com status {Status}";
	}
}