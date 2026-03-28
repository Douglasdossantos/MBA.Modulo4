using MBA.Core.Autentications;
using MBA.Core.DomainObjects;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Messages.FaturamentoCommands;
using MBA.Pagamentos.Api.ViewModels;
using MBA.WebApi.Core.Controllers;
using MBA.WebApi.Core.Identidade;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MBA.Pagamentos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FaturamentoController(IAppIdentityUser appIdentityUser,
    INotificationHandler<DomainNotificacaoRaiz> notifications,
    IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
{

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
}