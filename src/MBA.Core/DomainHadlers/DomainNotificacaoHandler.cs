using MBA.Core.Messages;
using MediatR;

namespace MBA.Core.DomainHadlers
{
    public class DomainNotificacaoHandler : INotificationHandler<DomainNotificacaoRaiz>
    {
        private readonly List<DomainNotificacaoRaiz> _notificacoes = [];
        public DomainNotificacaoHandler() { }

        public async Task Handle(DomainNotificacaoRaiz notificacao, CancellationToken cancellationToken)
        {
            _notificacoes.Add(notificacao);
            await Task.CompletedTask;
        }

        public List<DomainNotificacaoRaiz> ObterNotificacoes() => _notificacoes;
        public bool TemNotificacao() => _notificacoes.Count > 0;
        public void Limpar() => _notificacoes.Clear();
    }
}
