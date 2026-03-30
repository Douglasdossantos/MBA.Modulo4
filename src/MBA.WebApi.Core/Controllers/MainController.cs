using FluentValidation.Results;
using MBA.Core.Autentications;
using MBA.Core.DomainHadlers;
using MBA.Core.DomainObjects;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace MBA.WebApi.Core.Controllers
{
    [ApiController]
    public abstract class MainController(IAppIdentityUser appIdentityUser,
       INotificationHandler<DomainNotificacaoRaiz> notifications,
       IMediatorHandler mediatorHandler) : ControllerBase
    {
        private readonly IAppIdentityUser _appIdentityUser = appIdentityUser;
        protected readonly DomainNotificacaoHandler _notifications = (DomainNotificacaoHandler)notifications;
        protected readonly IMediatorHandler _mediatorHandler = mediatorHandler;

        protected bool OperacaoValida() => !_notifications.TemNotificacao();
        public Guid UserId => _appIdentityUser.ObterUsuarioId();
        public bool EstahAutenticado => _appIdentityUser.EstahAutenticado();
        public string Email => _appIdentityUser.ObterEmail();
        public bool EhAdministrador => _appIdentityUser.EhAdministrador();

        protected ICollection<string> Erros = [];

        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida())
            {
                return Ok(result);
            }

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", Erros.ToArray() }
            }));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                AdicionarErroProcessamento(erro.ErrorMessage);
            }

            return CustomResponse();
        }

        protected ActionResult CustomResponse(ValidationResult validationResult)
        {
            foreach (var erro in validationResult.Errors)
            {
                AdicionarErroProcessamento(erro.ErrorMessage);
            }

            return CustomResponse();
        }

        protected void AdicionarErroProcessamento(string erro)
        {
            Erros.Add(erro);
        }

        protected void LimparErrosProcessamento()
        {
            Erros.Clear();
        }

        protected ActionResult GenerateModelStateResponse(ResponseTypeEnum responseType, HttpStatusCode statusCode, ModelStateDictionary modelState)
        {
            return new JsonResult(new
            {
                success = false,
                type = responseType.ToString(),
                errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            })
            {
                StatusCode = (int)statusCode
            };
        }

        protected ActionResult GenerateResponse(object? result = null,
            ResponseTypeEnum responseType = ResponseTypeEnum.Success,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            IList<string> errors = null)
        {
            if (OperacaoValida() && ((int)statusCode >= 200 && (int)statusCode <= 299))
            {
                return new JsonResult(new
                {
                    success = true,
                    type = responseType.ToString(),
                    result
                })
                {
                    StatusCode = (int)statusCode
                };
            }

            errors ??= [];
            if (_notifications.TemNotificacao())
            {
                var notificationErrors = _notifications.ObterNotificacoes().Select(n => $"({n.Chave}: {n.RaizAgregacao}) Mensagem: {n.Valor}").ToList();
                foreach (string erro in notificationErrors)
                {
                    errors.Add(erro);
                }
            }

            return new JsonResult(new
            {
                success = false,
                type = responseType.ToString(),
                errors
            })
            {
                StatusCode = (int)statusCode
            };
        }

        protected ActionResult GenerateDomainExceptionResponse(object? result = null,
           ResponseTypeEnum responseType = ResponseTypeEnum.Success,
           HttpStatusCode statusCode = HttpStatusCode.OK,
           DomainException exception = null)
        {
            List<string> errors = [];
            if (exception != null)
            {
                errors.AddRange(exception.Errors ?? [exception.Message]);
            }

            return GenerateResponse(result, responseType, statusCode, errors);
        }
    }
}