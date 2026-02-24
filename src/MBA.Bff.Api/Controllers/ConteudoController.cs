using MBA.Bff.Api.Services.Interface;
using MBA.Core.Autentications;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Controllers
{
    [Authorize]
    public class ConteudoController(IConteudoService conteudoService,
    IAppIdentityUser appIdentityUser,
    INotificationHandler<DomainNotificacaoRaiz> notifications,
    IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
    {
        private readonly IConteudoService _conteudoService = conteudoService;

        [HttpGet]
        [Route("Conteudo/Exemple")]
        public async Task<IActionResult> Index()
        {
            //return CustomResponse(await Exemple.ObterExemple());
            return CustomResponse();
        }
    }
}