using FluentAssertions;

using MBA.Auth.Api.Extensions;

namespace MBA.Auth.Testes;

public class IdentityMensagensPortuguesTests
{
	private readonly IdentityMensagensPortugues _describer = new();

	[Fact]
	public void DefaultError_deve_retornar_mensagem_padrao_em_portugues()
	{
		var erro = _describer.DefaultError();

		erro.Code.Should().Be("DefaultError");
		erro.Description.Should().Be("Ocorreu um erro desconhecido.");
	}

	[Fact]
	public void PasswordMismatch_deve_retornar_senha_incorreta()
	{
		var erro = _describer.PasswordMismatch();

		erro.Code.Should().Be("PasswordMismatch");
		erro.Description.Should().Be("Senha incorreta.");
	}

	[Fact]
	public void InvalidToken_deve_retornar_token_invalido()
	{
		var erro = _describer.InvalidToken();

		erro.Code.Should().Be("InvalidToken");
		erro.Description.Should().Be("Token inválido.");
	}

	[Fact]
	public void DuplicateUserName_deve_interpolar_o_login()
	{
		var erro = _describer.DuplicateUserName("joao");

		erro.Code.Should().Be("DuplicateUserName");
		erro.Description.Should().Contain("joao").And.Contain("já está sendo utilizado");
	}

	[Fact]
	public void DuplicateEmail_deve_interpolar_o_email()
	{
		var erro = _describer.DuplicateEmail("teste@exemplo.com");

		erro.Code.Should().Be("DuplicateEmail");
		erro.Description.Should().Contain("teste@exemplo.com");
	}

	[Fact]
	public void InvalidEmail_deve_interpolar_o_email_e_marcar_como_invalido()
	{
		var erro = _describer.InvalidEmail("nao-eh-email");

		erro.Code.Should().Be("InvalidEmail");
		erro.Description.Should().Contain("nao-eh-email").And.Contain("inválido");
	}

	[Fact]
	public void InvalidUserName_deve_interpolar_o_login()
	{
		var erro = _describer.InvalidUserName("jo ao");

		erro.Code.Should().Be("InvalidUserName");
		erro.Description.Should().Contain("jo ao");
	}

	[Fact]
	public void PasswordTooShort_deve_incluir_o_tamanho_minimo()
	{
		var erro = _describer.PasswordTooShort(8);

		erro.Code.Should().Be("PasswordTooShort");
		erro.Description.Should().Contain("8");
	}

	[Fact]
	public void UserAlreadyInRole_deve_interpolar_a_permissao()
	{
		var erro = _describer.UserAlreadyInRole("Administrador");

		erro.Code.Should().Be("UserAlreadyInRole");
		erro.Description.Should().Contain("Administrador");
	}

	[Fact]
	public void DuplicateRoleName_deve_interpolar_a_permissao()
	{
		var erro = _describer.DuplicateRoleName("Gerente");

		erro.Code.Should().Be("DuplicateRoleName");
		erro.Description.Should().Contain("Gerente");
	}

	[Fact]
	public void PasswordRequiresDigit_deve_retornar_mensagem_em_portugues()
	{
		var erro = _describer.PasswordRequiresDigit();

		erro.Code.Should().Be("PasswordRequiresDigit");
		erro.Description.Should().Contain("digito");
	}

	[Fact]
	public void PasswordRequiresNonAlphanumeric_deve_retornar_mensagem_em_portugues()
	{
		var erro = _describer.PasswordRequiresNonAlphanumeric();

		erro.Code.Should().Be("PasswordRequiresNonAlphanumeric");
		erro.Description.Should().Contain("não alfanumérico");
	}
}
