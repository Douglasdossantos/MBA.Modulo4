using MediatR;

namespace MBA.Core.Messages
{
    public class DomainNotificacaoRaiz : INotification
    {
        public Guid RaizAgregacao { get; internal set; }
        public DateTime DataHora { get; private set; }
        public Guid NotificacaoId { get; private set; }
        public string Chave { get; private set; }
        public string Valor { get; private set; }

        public DomainNotificacaoRaiz(Guid raizAgregacao, string key, string value)
        {
            RaizAgregacao = raizAgregacao;
            DataHora = DateTime.Now;
            NotificacaoId = Guid.NewGuid();
            Chave = key;
            Valor = value;
        }
    }
}
