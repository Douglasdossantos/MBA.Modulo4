using FluentAssertions;

using MBA.Aluno.Domain.Entities;
using MBA.Core.DomainObjects;

namespace MBA.Aluno.Testes.Domains;

public class AulaAssistidaTests
{
	#region Helpers

	private static readonly Guid MatriculaCursoIdValido = Guid.NewGuid();
	private static readonly Guid AulaIdValido = Guid.NewGuid();

	private static AulaAssistida CriarAulaAssistida(
		Guid? matriculaCursoId = null,
		Guid? aulaId = null,
		DateTime? dataTermino = null)
		=> new(matriculaCursoId ?? MatriculaCursoIdValido, aulaId ?? AulaIdValido, dataTermino ?? DateTime.Now);

	#endregion

	[Fact]
	public void Deve_criar_aula_assistida_valida()
	{
		var aula = CriarAulaAssistida();

		aula.Should().NotBeNull();
		aula.MatriculaCursoId.Should().Be(MatriculaCursoIdValido);
		aula.AulaId.Should().Be(AulaIdValido);
		aula.DataTermino.Should().NotBe(default);
	}

	[Fact]
	public void Nao_deve_criar_com_matricula_curso_vazio()
	{
		Action act = () => CriarAulaAssistida(matriculaCursoId: Guid.Empty);

		act.Should().Throw<DomainException>()
			.WithMessage("*ID da matrícula do curso não pode estar vazio*");
	}

	[Fact]
	public void Nao_deve_criar_com_aula_vazia()
	{
		Action act = () => CriarAulaAssistida(aulaId: Guid.Empty);

		act.Should().Throw<DomainException>()
			.WithMessage("*ID da aula não pode estar vazio*");
	}

	[Fact]
	public void Nao_deve_criar_com_data_default()
	{
		Action act = () => CriarAulaAssistida(dataTermino: default(DateTime));

		act.Should().Throw<DomainException>();
	}

	[Fact]
	public void Deve_alterar_aula_para_valor_valido()
	{
		var aula = CriarAulaAssistida();
		var novaAula = Guid.NewGuid();

		aula.AlterarAulaId(novaAula);

		aula.AulaId.Should().Be(novaAula);
	}

	[Fact]
	public void Deve_alterar_data_termino()
	{
		var aula = CriarAulaAssistida();
		var novaData = new DateTime(2023, 5, 10);

		aula.AlterarDataTermino(novaData);

		aula.DataTermino.Should().Be(novaData);
	}

	[Fact]
	public void ToString_deve_conter_a_aula()
	{
		var aula = CriarAulaAssistida();

		aula.ToString().Should().Contain(AulaIdValido.ToString());
	}
}
