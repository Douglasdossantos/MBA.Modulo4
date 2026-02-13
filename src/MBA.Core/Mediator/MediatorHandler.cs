using FluentValidation.Results;
using MBA.Core.Messages;
using MediatR;
using SaberOnline.Core.Messages;

namespace MBA.Core.Mediator
{
    public class MediatorHandler : IMediatorHandler
    {
        private readonly IMediator _mediator;

        public MediatorHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ValidationResult> EnviarComando<T>(T comando) where T : Command
        {
            return await _mediator.Send(comando);
        }

       

        public async Task PublicarEvento<T>(T evento) where T : Event
        {
            await _mediator.Publish(evento);
        }


        //VERIFICAR ESSE CASO

        public async Task PublicarEventoRaiz<T>(T evento) where T : EventoRaiz
        {
            await _mediator.Publish(evento);
        }

        public async Task<bool> EnviarComandoRaiz<T>(T comando) where T : CommandRaiz
        {
            return await _mediator.Send(comando);
        }


        public async Task PublicarNotificacaoDominio<T>(T notificacao) where T : DomainNotificacaoRaiz
        {
            await _mediator.Publish(notificacao);
        }
    }
}