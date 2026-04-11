using FluentValidation.Results;
using MBA.Bff.Api.Services.Interface;
using MBA.Core.Messages.Integration;
using MBA.MessageBus;

namespace MBA.Bff.Api.Handlers
{
    public class AlterarStatusMatriculaIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;
        private bool _responderRegistrado = false;

        public AlterarStatusMatriculaIntegrationHandler(
            IServiceProvider serviceProvider,
            IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        private void SetResponder()
        {
            if (_responderRegistrado) return;

            _bus.RespondAsync<AlterarStatusMatriculaIntegrationEvent, ResponseMessage>(async request =>
            {
                var sucesso = await AlterarStastusMatricula(request);

                var validationResult = new ValidationResult();

                if (!sucesso)
                {
                    validationResult.Errors.Add(
                        new ValidationFailure("Erro", "Falha ao alterar status da matrícula"));
                }

                return new ResponseMessage(validationResult);
            });

            _responderRegistrado = true;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetResponder();

            _bus.AdvancedBus.Connected += (s, e) =>
            {
                _responderRegistrado = false;
                SetResponder();
            };

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private void OnConnect(object s, EventArgs e)
        {
            SetResponder();
        }

        private async Task<bool> AlterarStastusMatricula(AlterarStatusMatriculaIntegrationEvent message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var alunoService = scope.ServiceProvider.GetRequiredService<IAlunoService>();

                var response = await alunoService.AlterarStatusMatricula(
                    message.Id,
                    (int)message.Status,
                    ""
                );

                return response.StatusCode >= 200 && response.StatusCode < 300;
            }
        }
    }
}