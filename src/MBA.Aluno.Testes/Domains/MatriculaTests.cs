using FluentAssertions;

using MBA.Aluno.Domain.Entities;
using MBA.Core.DomainObjects;

using MatriculaDto = MBA.Core.SharedDto.Aluno.MatriculaDto;
using StatusMatricula = MBA.Core.SharedDto.Aluno.Enum.StatusMatricula;

namespace MBA.Aluno.Testes.Domains;

public class MatriculaTests
{
	#region Helpers

	private static readonly Guid CursoIdValido = Guid.NewGuid();
	private static readonly Guid AlunoIdValido = Guid.NewGuid();

	private static Matricula CriarMatricula(
		Guid? cursoId = null,
		Guid? alunoId = null,
		DateTime? dataMatricula = null,
		StatusMatricula status = StatusMatricula.PendentePagamento)
		=> new(cursoId ?? CursoIdValido, alunoId ?? AlunoIdValido, dataMatricula ?? DateTime.Now, status);

	#endregion

	#region Construtor

	[Fact]
	public void Deve_criar_matricula_valida()
	{
		var matricula = CriarMatricula();

		matricula.Should().NotBeNull();
		matricula.CursoId.Should().Be(CursoIdValido);
		matricula.AlunoId.Should().Be(AlunoIdValido);
		matricula.Status.Should().Be(StatusMatricula.PendentePagamento);
		matricula.DataCursoConcluido.Should().BeNull();
	}

	[Fact]
	public void Nao_deve_criar_matricula_com_curso_vazio()
	{
		Action act = () => CriarMatricula(cursoId: Guid.Empty);

		act.Should().Throw<DomainException>()
			.WithMessage("*ID do curso não pode ser vazio*");
	}

	[Fact]
	public void Nao_deve_criar_matricula_com_aluno_vazio()
	{
		Action act = () => CriarMatricula(alunoId: Guid.Empty);

		act.Should().Throw<DomainException>()
			.WithMessage("*ID do aluno não pode ser vazio*");
	}

	[Fact]
	public void Nao_deve_criar_matricula_com_data_default()
	{
		Action act = () => CriarMatricula(dataMatricula: default(DateTime));

		act.Should().Throw<DomainException>()
			.WithMessage("*data da matrícula é inválida*");
	}

	#endregion

	#region Transicoes de status

	[Fact]
	public void Deve_alterar_status_por_metodos()
	{
		var matricula = CriarMatricula();

		matricula.StatusPagamentoRealizado();
		matricula.Status.Should().Be(StatusMatricula.PagamentoRealizado);

		matricula.StatusConcluido();
		matricula.Status.Should().Be(StatusMatricula.Concluido);

		matricula.StatusCancelada();
		matricula.Status.Should().Be(StatusMatricula.Cancelada);

		matricula.StatusPendentePagamento();
		matricula.Status.Should().Be(StatusMatricula.PendentePagamento);
	}

	[Theory]
	[InlineData(1, StatusMatricula.PendentePagamento)]
	[InlineData(2, StatusMatricula.PagamentoRealizado)]
	[InlineData(3, StatusMatricula.Concluido)]
	[InlineData(4, StatusMatricula.Cancelada)]
	public void Deve_alterar_status_por_codigo(int codigo, StatusMatricula esperado)
	{
		var matricula = CriarMatricula();

		matricula.AlterarStatusPorCodigo(codigo);

		matricula.Status.Should().Be(esperado);
	}

	[Fact]
	public void Deve_lancar_para_codigo_de_status_invalido()
	{
		var matricula = CriarMatricula();

		Action act = () => matricula.AlterarStatusPorCodigo(99);

		act.Should().Throw<ArgumentException>()
			.WithMessage("*Código de status inválido*");
	}

	#endregion

	#region Alteracoes e datas

	[Fact]
	public void Deve_alterar_curso_e_aluno_para_valores_validos()
	{
		var matricula = CriarMatricula();
		var novoCurso = Guid.NewGuid();
		var novoAluno = Guid.NewGuid();

		matricula.AlterarCursoId(novoCurso);
		matricula.AlterarAlunoId(novoAluno);

		matricula.CursoId.Should().Be(novoCurso);
		matricula.AlunoId.Should().Be(novoAluno);
	}

	[Fact]
	public void Deve_marcar_data_de_conclusao()
	{
		var matricula = CriarMatricula();

		matricula.CriarDataConcluido();

		matricula.DataCursoConcluido.Should().NotBeNull();
		matricula.DataCursoConcluido!.Value.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
	}

	#endregion

	#region Conversao para DTO

	[Fact]
	public void Deve_converter_matricula_para_dto()
	{
		var matricula = CriarMatricula();

		MatriculaDto dto = matricula;

		dto.Id.Should().Be(matricula.Id);
		dto.CursoId.Should().Be(matricula.CursoId);
		dto.AlunoId.Should().Be(matricula.AlunoId);
		dto.Status.Should().Be(matricula.Status);
	}

	[Fact]
	public void Deve_converter_matricula_nula_para_dto_vazio()
	{
		Matricula? matricula = null;
		MatriculaDto dto = matricula;

		dto.Should().NotBeNull();
	}

	#endregion
}
