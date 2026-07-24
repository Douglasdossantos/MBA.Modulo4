using FluentAssertions;

using FluentValidation.Results;

using MBA.Core.Messages;
using MBA.Core.Messages.Integration;

namespace MBA.Core.Testes;

public class MessagesTests
{
	private static ValidationResult ComErro(string mensagem)
		=> new(new[] { new ValidationFailure("Campo", mensagem) });

	[Fact]
	public void Command_novo_deve_ser_valido_e_sem_erros()
	{
		var comando = new FakeCommand();

		comando.EhValido().Should().BeTrue();
		comando.Erros.Should().BeEmpty();
	}

	[Fact]
	public void Command_deve_refletir_validacao_invalida()
	{
		var comando = new FakeCommand();

		comando.DefinirValidacao(ComErro("Erro do comando"));

		comando.EhValido().Should().BeFalse();
		comando.Erros.Should().Contain("Erro do comando");
	}

	[Fact]
	public void Command_deve_definir_raiz_de_agregacao()
	{
		var comando = new FakeCommand();
		var id = Guid.NewGuid();

		comando.DefinirRaizAgregacao(id);

		comando.RaizAgregacao.Should().Be(id);
	}

	[Fact]
	public void CommandRaiz_deve_expor_raiz_validacao_e_erros()
	{
		var comando = new FakeCommandRaiz();
		var id = Guid.NewGuid();

		comando.DefinirRaizAgregacao(id);
		comando.DefinirValidacao(ComErro("Erro raiz"));

		comando.RaizAgregacao.Should().Be(id);
		comando.EhValido().Should().BeFalse();
		comando.Erros.Should().Contain("Erro raiz");
	}

	[Fact]
	public void EventoRaiz_novo_deve_ser_valido_e_aceitar_raiz()
	{
		var evento = new EventoRaiz();
		var id = Guid.NewGuid();

		evento.DefinirRaizAgregacao(id);

		evento.EhValido().Should().BeTrue();
		evento.RaizAgregacao.Should().Be(id);
	}

	[Fact]
	public void DomainNotificacaoRaiz_deve_preencher_dados()
	{
		var id = Guid.NewGuid();

		var notificacao = new DomainNotificacaoRaiz(id, "chave", "valor");

		notificacao.RaizAgregacao.Should().Be(id);
		notificacao.Chave.Should().Be("chave");
		notificacao.Valor.Should().Be("valor");
		notificacao.NotificacaoId.Should().NotBe(Guid.Empty);
	}

	[Fact]
	public void Message_deve_expor_o_nome_do_tipo()
	{
		var comando = new FakeCommand();

		((Message)comando).MessageType.Should().Be(nameof(FakeCommand));
	}

	[Fact]
	public void ResponseMessage_deve_guardar_a_validacao()
	{
		var validacao = new ValidationResult();

		var resposta = new ResponseMessage(validacao);

		resposta.ValidationResult.Should().BeSameAs(validacao);
	}
}
