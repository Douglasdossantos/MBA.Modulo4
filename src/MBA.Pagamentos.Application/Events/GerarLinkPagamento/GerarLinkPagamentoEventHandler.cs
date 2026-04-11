using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.FaturamentoEvents;
using MBA.Pagamentos.Domain.Entities;
using MBA.Pagamentos.Domain.Interfaces;

using MediatR;

namespace MBA.Pagamentos.Application.Events.GerarLinkPagamento;

public class GerarLinkPagamentoEventHandler(
	IFaturamentoRepository faturamentoRepository,
	IMediatorHandler mediatorHandler) : INotificationHandler<GerarLinkPagamentoEvent>
{
	private Guid _raizAgregacao;

	public async Task Handle(GerarLinkPagamentoEvent request, CancellationToken cancellationToken)
	{
		_raizAgregacao = request.RaizAgregacao;
		if (!ValidarRequisicao(request)) return;

		var pagamento = new Pagamento(request.MatriculaCursoId, request.Valor, request.DataHora.AddDays(7).Date);
		await faturamentoRepository.AdicionarAsync(pagamento);

		await faturamentoRepository.UnitOfWork.Commit();
	}

	private bool ValidarRequisicao(GerarLinkPagamentoEvent notification)
	{
		notification.DefinirValidacao(new GerarLinkPagamentoEventValidator().Validate(notification));
		if (!notification.EhValido())
		{
			foreach (var erro in notification.Erros)
				mediatorHandler
					.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Pagamento), erro))
					.GetAwaiter().GetResult();
			return false;
		}

		return true;
	}
}