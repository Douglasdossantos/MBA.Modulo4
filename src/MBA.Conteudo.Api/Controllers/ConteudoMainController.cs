using MBA.Conteudo.Api.Enumerators;
using MBA.Conteudo.Api.ViewModels;
using MBA.Core.DomainObjects;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Controllers;
using MBA.WebApi.Core.Usuario;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SaberOnline.Core.Messages;
using System.Net;

namespace MBA.Conteudo.Api.Controllers
{
    public abstract class ConteudoMainController : MainController
    {
        private readonly IAspNetUser _aspNetUser;
        private readonly INotificationHandler<DomainNotificacaoRaiz> _notifications;
        private readonly IMediatorHandler _mediatorHandler;

        protected ConteudoMainController(
            IAspNetUser aspNetUser,
            INotificationHandler<DomainNotificacaoRaiz> notifications,
            IMediatorHandler mediatorHandler)
        {
            _aspNetUser = aspNetUser;
            _notifications = notifications;
            _mediatorHandler = mediatorHandler;
        }

        protected IActionResult GenerateResponse(object? data, ResponseTypeEnum responseType, HttpStatusCode statusCode, List<string>? errors = null)
        {
            var response = new ApiResponse<object>
            {
                Success = responseType == ResponseTypeEnum.Success,
                Data = data,
                Errors = errors ?? new List<string>(),
                StatusCode = (int)statusCode
            };

            return StatusCode((int)statusCode, response);
        }

        protected IActionResult GenerateModelStateResponse(ResponseTypeEnum responseType, HttpStatusCode statusCode, ModelStateDictionary modelState)
        {
            var errors = modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return GenerateResponse(null, responseType, statusCode, errors);
        }

        protected IActionResult GenerateDomainExceptionResponse(object data, ResponseTypeEnum responseType, HttpStatusCode statusCode, DomainException exception)
        {
            var errors = new List<string> { exception.Message };
            return GenerateResponse(data, responseType, statusCode, errors);
        }

        protected Guid UsuarioId => _aspNetUser.ObterUserId();
        protected string UsuarioEmail => _aspNetUser.ObterUserEmail();
        protected bool UsuarioAutenticado => _aspNetUser.EstaAutenticado();
    }
}
