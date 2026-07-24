using FluentAssertions;

using MBA.Core.Messages.Integration;

// O tipo MessageBus vive no namespace MBA.MessageBus; o alias evita a colisão
// entre o nome do tipo e o do namespace dentro de MBA.MessageBus.Testes.
using MessageBusSut = MBA.MessageBus.MessageBus;

namespace MBA.MessageBus.Testes;

public class FakeIntegrationEvent : IntegrationEvent { }

public class MessageBusTests
{
	private const string ConnValida = "host=localhost:5672;username=guest;password=guest";

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("   ")]
	public void Construtor_deve_exigir_connection_string(string? conn)
	{
		Action act = () => new MessageBusSut(conn!);

		act.Should().Throw<ArgumentException>()
			.WithMessage("*Connection string*");
	}

	[Fact]
	public void Deve_construir_com_connection_string_valida()
	{
		using var bus = new MessageBusSut(ConnValida);

		bus.Should().NotBeNull();
	}

	[Fact]
	public void IsConnected_deve_ser_falso_sem_conexao()
	{
		using var bus = new MessageBusSut(ConnValida);

		bus.IsConnected.Should().BeFalse();
	}

	[Fact]
	public void AdvancedBus_deve_lancar_quando_nao_conectado()
	{
		using var bus = new MessageBusSut(ConnValida);

		Action act = () => _ = bus.AdvancedBus;

		act.Should().Throw<InvalidOperationException>()
			.WithMessage("*not connected*");
	}

	[Fact]
	public void Publish_deve_lancar_para_mensagem_nula()
	{
		using var bus = new MessageBusSut(ConnValida);

		Action act = () => bus.Publish<FakeIntegrationEvent>(null!);

		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public async Task PublishAsync_deve_lancar_para_mensagem_nula()
	{
		using var bus = new MessageBusSut(ConnValida);

		Func<Task> act = () => bus.PublishAsync<FakeIntegrationEvent>(null!);

		await act.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public void Subscribe_deve_lancar_para_id_em_branco()
	{
		using var bus = new MessageBusSut(ConnValida);

		Action act = () => bus.Subscribe<object>("   ", _ => { });

		act.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void Subscribe_deve_lancar_para_handler_nulo()
	{
		using var bus = new MessageBusSut(ConnValida);

		Action act = () => bus.Subscribe<object>("assinatura", null!);

		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Dispose_sem_conexao_nao_deve_lancar()
	{
		var bus = new MessageBusSut(ConnValida);

		Action act = () => bus.Dispose();

		act.Should().NotThrow();
	}

	[Fact]
	public void Operacao_apos_dispose_deve_lancar_object_disposed()
	{
		var bus = new MessageBusSut(ConnValida);
		bus.Dispose();

		Action act = () => bus.Publish(new FakeIntegrationEvent());

		act.Should().Throw<ObjectDisposedException>();
	}
}
