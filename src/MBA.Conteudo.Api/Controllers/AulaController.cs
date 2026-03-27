using AutoMapper;
using MBA.API.ViewModels;
using MBA.Conteudo.Application.Services;
using MBA.Core.Autentications;
using MBA.Core.DomainObjects;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.SharedDto;
using MBA.WebApi.Core.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MBA.Conteudo.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AulaController(IAulaAppService aulaAppService,
    IMapper mapper,
    IAppIdentityUser appIdentityUser,
    INotificationHandler<DomainNotificacaoRaiz> notifications,
    IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
{
    private readonly IAulaAppService _aulaAppService = aulaAppService;
    private readonly IMapper _mapper = mapper;

    //[ClaimsAuthorize("Aulas", "AD")]
    [HttpPost("{cursoId}")]
    public async Task<IActionResult> AdicionarAula(Guid cursoId, [FromBody] AulaViewModel aulaViewModel)
    {
        if (!ModelState.IsValid) { return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState); }
        if (cursoId != aulaViewModel.CursoId) { return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden, ["Você não tem permissão para realizar essa operação. Verifique sua requisição"]); }

        try
        {
            var dto = _mapper.Map<AulaDto>(aulaViewModel);
            var aulaId = await _aulaAppService.AdicionarAulaAsync(cursoId, dto);
            return GenerateResponse(new { AulaId = aulaId }, ResponseTypeEnum.Success, HttpStatusCode.Created);
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

   // [ClaimsAuthorize("Aulas", "AT")]
    [HttpPut("{cursoId}")]
    public async Task<IActionResult> AtualizarAula(Guid cursoId, [FromBody] AulaViewModel aulaViewModel)
    {
        if (!ModelState.IsValid) { return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState); }
        if (cursoId != aulaViewModel.CursoId) { return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden, ["Você não tem permissão para realizar essa operação. Verifique sua requisição"]); }

        try
        {
            var dto = _mapper.Map<AulaDto>(aulaViewModel);
            await _aulaAppService.AtualizarAulaAsync(cursoId, dto);
            return GenerateResponse(null, ResponseTypeEnum.Success, HttpStatusCode.NoContent);
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

    ///[ClaimsAuthorize("Aulas", "RM")]
    [HttpDelete("{cursoId}/remover/{aulaId}")]
    public async Task<IActionResult> RemoverAula(Guid cursoId, Guid aulaId)
    {
        try
        {
            await _aulaAppService.RemoverAulaAsync(cursoId, aulaId);
            return GenerateResponse(null, ResponseTypeEnum.Success, HttpStatusCode.NoContent);
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