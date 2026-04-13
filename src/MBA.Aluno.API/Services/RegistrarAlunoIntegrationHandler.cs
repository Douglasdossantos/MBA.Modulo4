using FluentValidation.Results;

using MBA.Core.Mediator;
using MBA.Core.Messages.AlunoCommands;
using MBA.Core.Messages.Integration;
using MBA.MessageBus;

namespace MBA.Aluno.API.Services;

public class CadastroAlunoIntegrationHandler : BackgroundService
{
	private readonly IMessageBus _bus;
	private readonly IServiceProvider _serviceProvider;

	public CadastroAlunoIntegrationHandler(
		IServiceProvider serviceProvider,
		IMessageBus bus)
	{
		_serviceProvider = serviceProvider;
		_bus = bus;
	}

	private void SetResponder()
	{
		_bus.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request =>
			await RegistrarCliente(request));

		_bus.AdvancedBus.Connected += OnConnect;
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		SetResponder();
		return Task.CompletedTask;
	}

	private void OnConnect(object? s, EventArgs e)
	{
		SetResponder();
	}

	private async Task<ResponseMessage> RegistrarCliente(UsuarioRegistradoIntegrationEvent message)
	{
		var clienteCommand = new CadastroAlunoCommand(message.Id, message.Nome, message.Email, true,
			message.Administrador, DateTime.Now);
		ValidationResult sucesso;

		using (var scope = _serviceProvider.CreateScope())
		{
			var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
			sucesso = await mediator.EnviarComando(clienteCommand);
		}

		return new ResponseMessage(sucesso);
	}
}