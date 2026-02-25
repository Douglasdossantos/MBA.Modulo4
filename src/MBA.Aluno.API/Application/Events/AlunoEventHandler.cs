using MediatR;

namespace MBA.Aluno.API.Application.Events
{
    public class AlunoEventHandler : INotificationHandler<AlunoRegistradoEvent>
    {
        public Task Handle(AlunoRegistradoEvent notification, CancellationToken cancellationToken)
        {
            // enviar um evento de confirmação
            return Task.CompletedTask;
        }
    }
}
