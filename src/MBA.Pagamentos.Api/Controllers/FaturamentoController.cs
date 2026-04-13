using MBA.Core.Autentications;
using MBA.Core.DomainObjects;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.DomainHadlers;
using MBA.Messages.FaturamentoCommands;
using MBA.Pagamentos.Api.ViewModels;
using MBA.Pagamentos.Application.Queries.Dtos;
using MBA.Pagamentos.Application.Queries.ObterPagamento;
using MBA.WebApi.Core.Controllers;
using MBA.WebApi.Core.Identidade;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MBA.Pagamentos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FaturamentoController(IAppIdentityUser appIdentityUser,
    INotificationHandler<DomainNotificacaoRaiz> notifications,
    IMediatorHandler mediatorHandler,
    IMediator mediator,
    ILogger<FaturamentoController> logger) : MainController(appIdentityUser, notifications, mediatorHandler)
{
    private readonly ILogger<FaturamentoController> _logger = logger;
    private readonly IMediator _mediator = mediator;

    [ClaimsAuthorize("Administrador", "PG")]
    [ClaimsAuthorize("Alunos", "PG")]
    [HttpPost("{alunoId}/registrar-pagamento")]
    public async Task<IActionResult> RealizarPagamento(Guid alunoId, RealizarPagamentoViewModel pagamentoViewModel)
    {
        if (!ModelState.IsValid) { return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState); }

        try
        {
           
            if (pagamentoViewModel.MatriculaCursoId == default) { return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.NotFound, ["Matrícula do curso não encontrada"]); }

            var comando = new RealizarPagamentoCommand(
                pagamentoViewModel.MatriculaCursoId,
                pagamentoViewModel.CursoId,
                pagamentoViewModel.AlunoId,
                pagamentoViewModel.PagamentoPodeSerRealizado,
                pagamentoViewModel.NomeCurso,
                pagamentoViewModel.Valor,
                pagamentoViewModel.DataMatricula,
                pagamentoViewModel.DataConclusao,
                pagamentoViewModel.EstadoMatricula,
                pagamentoViewModel.NumeroCartao,
                pagamentoViewModel.NomeTitularCartao,
                pagamentoViewModel.ValidadeCartao,
                pagamentoViewModel.CvvCartao
            );

            var sucesso = await _mediatorHandler.EnviarComandoRaiz(comando);
            if (sucesso)
            {
                return GenerateResponse(new { pagamentoViewModel.AlunoId, pagamentoViewModel.MatriculaCursoId },
                    responseType: ResponseTypeEnum.Success,
                    statusCode: HttpStatusCode.Created);
            }

            // If mediator returned false, gather domain notifications for details
            try
            {
                var handler = (DomainNotificacaoHandler)notifications;
                if (handler != null && handler.TemNotificacao())
                {
                    var errors = handler.ObterNotificacoes().Select(n => n.Valor).ToList();
                    _logger?.LogWarning("Pagamento command failed: {Errors}", string.Join("; ", errors));
                    return GenerateResponse(responseType: ResponseTypeEnum.ValidationError, statusCode: HttpStatusCode.BadRequest, errors: errors);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while reading domain notifications after command failure");
            }

            return GenerateResponse(responseType: ResponseTypeEnum.GenericError, statusCode: HttpStatusCode.BadRequest);
        }
        catch (DomainException exDomain)
        {
            return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
        }
        catch (Exception ex)
        {
            return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, [ex.Message]);
        }
    }

    /// <summary>
    /// Obtém o status de pagamento pelo identificador da matrícula.
    /// </summary>
    [ClaimsAuthorize("Administrador", "PG")]
    [ClaimsAuthorize("Alunos", "PG")]
    [HttpGet("matricula/{matriculaId:guid}")]
    [ProducesResponseType(typeof(PagamentoStatusDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> ObterPorMatricula(Guid matriculaId, CancellationToken cancellationToken)
    {
        var pagamento = await _mediator.Send(new ObterPagamentoPorMatriculaQuery(matriculaId), cancellationToken);

        if (pagamento is null)
        {
            return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.NotFound,
                ["Pagamento não encontrado para a matrícula informada."]);
        }

        return GenerateResponse(pagamento, ResponseTypeEnum.Success, HttpStatusCode.OK);
    }

    /// <summary>
    /// Obtém o status de pagamento pelo identificador do pagamento.
    /// </summary>
    [ClaimsAuthorize("Administrador", "PG")]
    [ClaimsAuthorize("Alunos", "PG")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PagamentoStatusDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var pagamento = await _mediator.Send(new ObterPagamentoPorIdQuery(id), cancellationToken);

        if (pagamento is null)
        {
            return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.NotFound,
                ["Pagamento não encontrado."]);
        }

        return GenerateResponse(pagamento, ResponseTypeEnum.Success, HttpStatusCode.OK);
    }
}