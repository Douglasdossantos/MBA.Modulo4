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
    public class AlunoController(
    IAlunoExternalService alunoService,
    IAppIdentityUser appIdentityUser,
    INotificationHandler<DomainNotificacaoRaiz> notifications,
    IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
    {
        
        private readonly IAlunoExternalService _alunoService = alunoService;

        [HttpGet]
        [Route("Aluno/Exemple")]
        public async Task<IActionResult> Index()
        {
            //return CustomResponse(await Exemple.ObterExemple());
            return CustomResponse();
        }
    }
}