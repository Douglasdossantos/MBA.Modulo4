using MBA.Bff.Api.Services.Interface;
using MBA.Core.Autentications;
using MBA.Core.DomainObjects;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MBA.Bff.Api.Controllers
{
    [Authorize]
    public class ConteudoController(IConteudoService conteudoService,
    IAppIdentityUser appIdentityUser,
    INotificationHandler<DomainNotificacaoRaiz> notifications,
    IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
    {
        private readonly IConteudoService _conteudoService = conteudoService;

         
        [HttpGet("{aulaId}")]
        public async Task<IActionResult> ObterAulaPorId(Guid aulaId)
        {
            try
            {
                var aula = await _conteudoService.ObterAulaPorId(aulaId);
                return GenerateResponse(aula, ResponseTypeEnum.Success, HttpStatusCode.OK);
            }
            catch (DomainException exDomain)
            {
                return GenerateDomainExceptionResponse("", ResponseTypeEnum.DomainError, HttpStatusCode.NotFound, exDomain);
            }
            catch (Exception ex)
            {
                return GenerateResponse("", ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, [ex.Message]);
            }
        }
    }
}