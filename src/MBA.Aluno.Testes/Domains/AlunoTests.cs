using FluentAssertions;

using MBA.Aluno.Domain.Entities;
using MBA.Core.DomainObjects;

using AlunoDto = MBA.Core.SharedDto.Aluno.AlunoDto;
using AlunoEntidade = MBA.Aluno.Domain.Entities.Aluno;

namespace MBA.Aluno.Testes.Domains;

public class AlunoTests
{
	#region Helpers

	private static readonly Guid IdValido = Guid.NewGuid();
	private const string NomeValido = "Aluno de Teste";
	private const string EmailValido = "aluno@teste.com";

	private static AlunoEntidade CriarAluno(
		Guid? id = null,
		string nome = NomeValido,
		string email = EmailValido,
		bool ativo = true,
		bool adm = false,
		DateTime? dataCriacao = null)
		=> new(id ?? IdValido, nome, email, ativo, adm, dataCriacao ?? DateTime.Now);

	#endregion

	#region Construtor

	[Fact]
	public void Deve_criar_aluno_valido()
	{
		var aluno = CriarAluno();

		aluno.Should().NotBeNull();
		aluno.Id.Should().Be(IdValido);
		aluno.Nome.Should().Be(NomeValido);
		aluno.Email.Should().Be(EmailValido);
		aluno.Ativo.Should().BeTrue();
		aluno.Adm.Should().BeFalse();
		aluno.DataCriacao.Should().NotBe(default);
		aluno.Matriculas.Should().BeEmpty();
	}

	[Fact]
	public void Nao_deve_criar_aluno_com_id_vazio()
	{
		Action act = () => CriarAluno(id: Guid.Empty);

		act.Should().Throw<DomainException>()
			.WithMessage("*Id do aluno não pode estar vazio*");
	}

	[Theory]
	[InlineData("", "*nome do aluno não pode estar vazio*")]
	[InlineData("ab", "*nome deve ter entre 3 e 150 caracteres*")]
	public void Nao_deve_criar_aluno_com_nome_invalido(string nome, string mensagem)
	{
		Action act = () => CriarAluno(nome: nome);

		act.Should().Throw<DomainException>().WithMessage(mensagem);
	}

	[Theory]
	[InlineData("", "*email não pode estar vazio*")]
	[InlineData("a@b", "*email deve ter entre 5 e 200 caracteres*")]
	public void Nao_deve_criar_aluno_com_email_invalido(string email, string mensagem)
	{
		Action act = () => CriarAluno(email: email);

		act.Should().Throw<DomainException>().WithMessage(mensagem);
	}

	[Fact]
	public void Nao_deve_criar_aluno_com_data_criacao_default()
	{
		Action act = () => CriarAluno(dataCriacao: default(DateTime));

		act.Should().Throw<DomainException>()
			.WithMessage("*data de criação é inválida*");
	}

	#endregion

	#region Comportamentos

	[Fact]
	public void Deve_alterar_nome_quando_valido()
	{
		var aluno = CriarAluno();

		aluno.AlterarNome("Nome Alterado");

		aluno.Nome.Should().Be("Nome Alterado");
	}

	[Fact]
	public void Nao_deve_alterar_nome_para_valor_muito_curto()
	{
		var aluno = CriarAluno();

		Action act = () => aluno.AlterarNome("ab");

		act.Should().Throw<DomainException>()
			.WithMessage("*nome deve ter entre 3 e 150 caracteres*");
	}

	[Fact]
	public void Deve_alterar_email_quando_valido()
	{
		var aluno = CriarAluno();

		aluno.AlterarEmail("novo@teste.com");

		aluno.Email.Should().Be("novo@teste.com");
	}

	[Fact]
	public void Nao_deve_alterar_email_para_valor_muito_curto()
	{
		var aluno = CriarAluno();

		Action act = () => aluno.AlterarEmail("a@b");

		act.Should().Throw<DomainException>()
			.WithMessage("*email deve ter entre 5 e 200 caracteres*");
	}

	[Fact]
	public void Deve_ativar_e_desativar()
	{
		var aluno = CriarAluno(ativo: false);

		aluno.Ativar();
		aluno.Ativo.Should().BeTrue();

		aluno.Desativar();
		aluno.Ativo.Should().BeFalse();
	}

	[Fact]
	public void Deve_definir_adm()
	{
		var aluno = CriarAluno(adm: false);

		aluno.DefinirAdm(true);

		aluno.Adm.Should().BeTrue();
	}

	[Fact]
	public void Deve_criar_data_com_data_atual()
	{
		var aluno = CriarAluno();

		aluno.CriarData();

		aluno.DataCriacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
	}

	[Fact]
	public void Deve_manter_a_mesma_data_informada()
	{
		var aluno = CriarAluno();
		var data = new DateTime(2020, 1, 1);

		aluno.CriarDataDeixaAMesma(data);

		aluno.DataCriacao.Should().Be(data);
	}

	#endregion

	#region Conversao para DTO

	[Fact]
	public void Deve_converter_aluno_para_dto()
	{
		var aluno = CriarAluno();

		AlunoDto dto = aluno;

		dto.Id.Should().Be(aluno.Id);
		dto.Nome.Should().Be(aluno.Nome);
		dto.Email.Should().Be(aluno.Email);
		dto.Ativo.Should().Be(aluno.Ativo);
		dto.Adm.Should().Be(aluno.Adm);
	}

	[Fact]
	public void Deve_converter_aluno_nulo_para_dto_vazio()
	{
		AlunoEntidade? aluno = null;
		AlunoDto dto = aluno;

		dto.Should().NotBeNull();
		dto.Nome.Should().BeEmpty();
	}

	[Fact]
	public void ToString_deve_conter_o_nome()
	{
		var aluno = CriarAluno();

		aluno.ToString().Should().Contain(NomeValido);
	}

	#endregion
}
