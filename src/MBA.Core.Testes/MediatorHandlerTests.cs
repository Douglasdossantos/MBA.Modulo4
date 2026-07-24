using FluentAssertions;

using FluentValidation.Results;

using MBA.Core.Mediator;
using MBA.Core.Messages;

using MediatR;

using Moq;

namespace MBA.Core.Testes;

public class MediatorHandlerTests
{
	private readonly Mock<IMediator> _mediator = new();
	private readonly MediatorHandler _handler;

	public MediatorHandlerTests() => _handler = new MediatorHandler(_mediator.Object);

	[Fact]
	public async Task EnviarComando_deve_delegar_para_send_e_retornar_o_resultado()
	{
		var esperado = new ValidationResult();
		_mediator
			.Setup(m => m.Send(It.IsAny<IRequest<ValidationResult>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(esperado);
		var comando = new FakeCommand();

		var resultado = await _handler.EnviarComando(comando);

		resultado.Should().BeSameAs(esperado);
		_mediator.Verify(m => m.Send(comando, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task EnviarComandoRaiz_deve_delegar_para_send_e_retornar_bool()
	{
		_mediator
			.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);
		var comando = new FakeCommandRaiz();

		var resultado = await _handler.EnviarComandoRaiz(comando);

		resultado.Should().BeTrue();
		_mediator.Verify(m => m.Send(comando, It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task PublicarEvento_deve_delegar_para_publish()
	{
		var evento = new FakeEvent();

		await _handler.PublicarEvento(evento);

		_mediator.Verify(m => m.Publish(It.IsAny<FakeEvent>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task PublicarEventoRaiz_deve_delegar_para_publish()
	{
		var evento = new EventoRaiz();

		await _handler.PublicarEventoRaiz(evento);

		_mediator.Verify(m => m.Publish(It.IsAny<EventoRaiz>(), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task PublicarNotificacaoDominio_deve_delegar_para_publish()
	{
		var notificacao = new DomainNotificacaoRaiz(Guid.NewGuid(), "chave", "valor");

		await _handler.PublicarNotificacaoDominio(notificacao);

		_mediator.Verify(m => m.Publish(It.IsAny<DomainNotificacaoRaiz>(), It.IsAny<CancellationToken>()), Times.Once);
	}
}
