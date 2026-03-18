using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.FaturamentoEvents;
using MBA.Messages.FaturamentoCommands;
using MBA.Pagamentos.Domain.Entities;
using MBA.Pagamentos.Domain.ValueObjects;
using MediatR;
using SaberOnline.Application.Application.Commands.RealizarPagamento;


namespace SaberOnline.Application.Commands.RealizarPagamento;
public class RealizarPagamentoCommandHandler(IFaturamentoRepository faturamentoRepository,
    IMediatorHandler mediatorHandler) : IRequestHandler<RealizarPagamentoCommand, bool>
{
    private readonly IFaturamentoRepository _faturamentoRepository = faturamentoRepository;
    private readonly IMediatorHandler _mediatorHandler = mediatorHandler;
    private Guid _raizAgregacao;

    public async Task<bool> Handle(
     RealizarPagamentoCommand request,
     CancellationToken cancellationToken)
    {
        _raizAgregacao = request.RaizAgregacao;

        // 1️⃣ Validação do command
        if (!ValidarRequisicaoAsync(request))
            return false;

        // 2️⃣ Matrícula obrigatória
        if (request.MatriculaCursoId == Guid.Empty)
        {
            await _mediatorHandler.PublicarNotificacaoDominio(
                new DomainNotificacaoRaiz(
                    _raizAgregacao,
                    nameof(Pagamento),
                    "Matrícula inválida para realização de pagamento."));
            return false;
        }

        // 3️⃣ Busca pagamento (PODE ser null)
        var resultado = await ObterPagamentoMatriculaCurso(request.MatriculaCursoId);

        if (!resultado.Sucesso)
            return false;

        var pagamento = resultado.Pagamento;

        // 4️⃣ BLOQUEIO SOMENTE se já estiver APROVADO
        if (pagamento is not null && pagamento.PossuiPagamentoAprovado())
        {
            await _mediatorHandler.PublicarNotificacaoDominio(
                new DomainNotificacaoRaiz(
                    _raizAgregacao,
                    nameof(Pagamento),
                    "Pagamento desta matrícula já se encontra aprovado."));
            return false;
        }

        // 5️⃣ Validação de valor
        var valorReferencia = pagamento?.Valor ?? request.Valor;

        if (!ValidarValorPagamentoMatriculaCurso(request.Valor, valorReferencia))
            return false;

        // 6️⃣ Dados do cartão
        var dadosCartao = new DadosCartao(
            request.NumeroCartao,
            request.NomeTitularCartao,
            request.ValidadeCartao,
            request.CvvCartao);

        // 7️⃣ Criação ou reaproveitamento
        if (pagamento == null)
        {
            pagamento = new Pagamento(
                request.MatriculaCursoId,
                request.Valor,
                DateTime.Now.Date);

            await _faturamentoRepository.AdicionarAsync(pagamento);
        }

        // 8️⃣ Confirma pagamento (DOMÍNIO decide)
        pagamento.ConfirmarPagamento(
            DateTime.Now,
            Guid.NewGuid().ToString(),
            dadosCartao);

        // 9️⃣ Commit
        await _faturamentoRepository.UnitOfWork.Commit();

        // 🔟 Evento de domínio
        await _mediatorHandler.PublicarEventoRaiz(
            new PagamentoConfirmadoEvent(
                request.MatriculaCursoId,
                request.AlunoId,
                request.CursoId,
                true));

        return true;
    }

    private bool ValidarRequisicaoAsync(RealizarPagamentoCommand request)
    {
        request.DefinirValidacao(new RealizarPagamentoCommandValidator().Validate(request));
        if (!request.EhValido())
        {
            foreach (var erro in request.Erros)
            {
                _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Pagamento), erro)).GetAwaiter().GetResult();
            }
            return false;
        }

        return true;
    }

    private async Task<(bool Sucesso, Pagamento? Pagamento)>
    ObterPagamentoMatriculaCurso(Guid matriculaId)
    {
        var pagamento =
            await _faturamentoRepository.ObterPorMatriculaIdAsync(matriculaId);

        if (pagamento != null && pagamento.PossuiPagamentoAprovado())
        {
            await _mediatorHandler.PublicarNotificacaoDominio(
                new DomainNotificacaoRaiz(
                    _raizAgregacao,
                    nameof(Pagamento),
                    "Pagamento desta matrícula já se encontra paga"
                )
            );

            return (false, pagamento);
        }

        return (true, pagamento);
    }

    private bool ValidarValorPagamentoMatriculaCurso(decimal valorInformado, decimal valorMatricula)
    {
        if (valorInformado != valorMatricula)
        {
            _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Pagamento), "Valor de pagamento diverge do valor desta matricula")).GetAwaiter().GetResult();
            return false;
        }

        return true;
    }
}