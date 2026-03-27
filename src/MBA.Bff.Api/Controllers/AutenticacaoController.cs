using MBA.Bff.Api.Models.Autenticacao;
using MBA.Bff.Api.Services.Interface;
using MBA.Core.Autentications;
using MBA.Core.DomainObjects;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MBA.Bff.Api.Controllers
{
    [Route("api/[controller]")]
    public class AutenticacaoController(IAutenticacaoService autenticacao,
                                        IAppIdentityUser appIdentityUser,
                                        INotificationHandler<DomainNotificacaoRaiz> notifications,
                                        IMediatorHandler mediatorHandler) : 
                                                   MainController(appIdentityUser, notifications, mediatorHandler)
    {
        private readonly IAutenticacaoService _autenticacao = autenticacao;

        [HttpPost("autenticar")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginViewModel usuarioLogin)
        {
            try
            {
                var ResultLogin = await _autenticacao.Login(usuarioLogin);
                
                if (ResultLogin != null)
                {
                    string content = null;

                    if (ResultLogin is ContentResult cr)
                    {
                        content = cr.Content;
                    }
                    else if (ResultLogin is ObjectResult orr)
                    {
                        if (orr.Value is string s) content = s;
                        else content = JsonSerializer.Serialize(orr.Value);
                    }
                    else if (ResultLogin is JsonResult jr)
                    {
                        content = JsonSerializer.Serialize(jr.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        using var doc = JsonDocument.Parse(content);
                        if (doc.RootElement.TryGetProperty("accessToken", out var tokenProp))
                        {
                            string accessToken = tokenProp.GetString();
                        }
                        else
                        {
                            // accessToken not present - treat as auth failure
                            return GenerateResponse(ResultLogin, ResponseTypeEnum.GenericError, HttpStatusCode.Unauthorized);
                        }
                    }
                    else
                    {
                        return GenerateResponse(ResultLogin, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError);
                    }
                }

                return GenerateResponse(ResultLogin, ResponseTypeEnum.Success, HttpStatusCode.OK);
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