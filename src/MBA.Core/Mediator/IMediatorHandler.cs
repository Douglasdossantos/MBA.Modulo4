using FluentValidation.Results;
using MBA.Core.Messages;
using SaberOnline.Core.Messages;


namespace MBA.Core.Mediator
{
    public interface IMediatorHandler
    {
        Task PublicarEvento<T>(T evento) where T : Event;
        Task<ValidationResult> EnviarComando<T>(T comando) where T : Command;
        


        //VERIFICAR ESTE CASO
        Task PublicarEventoRaiz<T>(T evento) where T : EventoRaiz;


        Task<bool> EnviarComandoRaiz<T>(T comando) where T : CommandRaiz;

        Task PublicarNotificacaoDominio<T>(T notificacao) where T : DomainNotificacaoRaiz;

    }
}
