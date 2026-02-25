
using EasyNetQ;
using FluentValidation.Results;
using MBA.Aluno.API.Application.Commands;
using MBA.Core.Mediator;
using MBA.Core.Messages.Integration;
using Microsoft.OpenApi.Writers;

namespace MBA.Aluno.API.Services
{
    public class RegistroAlunoIntegrationHandler : BackgroundService
    {
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public RegistroAlunoIntegrationHandler(IServiceProvider serviceProvider, IBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _bus.Rpc.RespondAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(async request => 
            new ResponseMessage(await RegstarAluno(request)));
            return Task.CompletedTask;
        }

        private async Task<ValidationResult> RegstarAluno(UsuarioRegistradoIntegrationEvent message)
        {
            var alunoCommand = new RegistarAlunoCommand(message.Id, message.CriadoEm);
            ValidationResult sucesso;

            using(var scope = _serviceProvider.CreateScope())
            {
                 var mediator = scope.ServiceProvider.GetRequiredService<IMediatorHandler>();
                 sucesso = await mediator.EnviarComando(alunoCommand);
            }
            return sucesso;
        }
    }
}
