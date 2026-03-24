using MBA.Conteudo.Api.Enumerators;
using MBA.Conteudo.Api.ViewModels;
using MBA.Core.Autentications;
using MBA.Core.DomainObjects;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Net;

namespace MBA.Conteudo.Api.Controllers
{
    public abstract class ConteudoMainController(
        IAppIdentityUser appIdentityUser,
        INotificationHandler<DomainNotificacaoRaiz> notifications,
        IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
    {

        protected IActionResult GenerateResponse(object data, ResponseTypeEnum responseType, HttpStatusCode statusCode, List<string>? errors = null)
        {
            var response = new ApiResponse<object>
            {
                Success = responseType == ResponseTypeEnum.Success,
                Data = data,
                Errors = errors ?? [],
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

            return GenerateResponse(new object(), responseType, statusCode, errors);
        }

        protected IActionResult GenerateDomainExceptionResponse(object data, ResponseTypeEnum responseType, HttpStatusCode statusCode, DomainException exception)
        {
            var errors = new List<string> { exception.Message };
            return GenerateResponse(data, responseType, statusCode, errors);
        }
    }
}
