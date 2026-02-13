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

    public async Task<bool> Handle(RealizarPagamentoCommand request, CancellationToken cancellationToken)
    {
        _raizAgregacao = request.RaizAgregacao;

        if (!ValidarRequisicaoAsync(request))
            return false;

        if (!ObterPagamentoMatriculaCurso(request.MatriculaCursoId, out Pagamento pagamento))
            return false;

        if (!ValidarValorPagamentoMatriculaCurso(request.Valor, pagamento?.Valor ?? request.MatriculaCursoDto.Valor))
            return false;

        if (!request.MatriculaCursoDto.PagamentoPodeSerRealizado)
        {
            await _mediatorHandler.PublicarNotificacaoDominio(
                new DomainNotificacaoRaiz(_raizAgregacao, nameof(Pagamento),
                "Matricula não permite pagamento. Entre em contato com nosso SAC"));
            return false;
        }

        bool ehInclusaoPagamento = pagamento == null;

        var dadosCartao = new DadosCartao(
            request.NumeroCartao,
            request.NomeTitularCartao,
            request.ValidadeCartao,
            request.CvvCartao);

       
        if (ehInclusaoPagamento)
        {
            pagamento = new Pagamento(request.MatriculaCursoId, request.Valor, DateTime.Now.Date);
            await _faturamentoRepository.AdicionarAsync(pagamento);
        }

        
        await _faturamentoRepository.UnitOfWork.Commit();

        await _mediatorHandler.PublicarEventoRaiz(new PagamentoConfirmadoEvent(
         request.MatriculaCursoId,
         request.MatriculaCursoDto.AlunoId,
         request.MatriculaCursoDto.CursoId,
         true ));


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

    private bool ObterPagamentoMatriculaCurso(Guid matriculaId, out Pagamento pagamento)
    {
        pagamento = _faturamentoRepository.ObterPorMatriculaIdAsync(matriculaId).Result;

        if (pagamento != null)
        {
            if (pagamento.PossuiPagamentoAprovado())
            {
                _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Pagamento), "Pagamento desta Matricula já se encontra paga")).GetAwaiter().GetResult();
                return false;
            }
        }

        return true;
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