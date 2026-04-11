using MBA.Bff.Api.Models.Aluno;
using MBA.Bff.Api.Response;
using MBA.Bff.Api.Services.Interface;
using MBA.Core.Autentications;
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
    public class AlunoController(IAutenticacaoService autenticacao,
                                 IAlunoService alunoService,
                                 IAppIdentityUser appIdentityUser,
                                 INotificationHandler<DomainNotificacaoRaiz> notifications,
                                 IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
    {
        private readonly IAutenticacaoService _autenticacao = autenticacao;
        private readonly IAlunoService _alunoService = alunoService;

        [HttpPost("matricula-pagamento")]
        public async Task<IActionResult> MatriculaPagamento([FromBody] MatriculaViewModel matriculaViewModel)
        {

            try
            {
                var ResultLogin = await _autenticacao.Login(matriculaViewModel.Login);

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
                            JsonSerializerOptions options = new()
                            {
                                PropertyNameCaseInsensitive = true
                            };

                            var result = JsonSerializer.Deserialize<LoginResponse>(content, options);

                            matriculaViewModel.Login.AlunoId = Guid.Parse(result.UsuarioToken.Id);


                            // pass token to service if needed (example shows calling service after auth)
                            var conteudo = await _alunoService.MatriculaPagamento(matriculaViewModel, accessToken);
                            return GenerateResponse(((ContentResult)conteudo).Content, ResponseTypeEnum.Success, HttpStatusCode.OK);
                        }
                        else
                        {
                            // accessToken not present - treat as auth failure
                            return GenerateResponse(ResultLogin, ResponseTypeEnum.GenericError, HttpStatusCode.Unauthorized);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return CustomResponse();
        }

        [HttpPost("realizar-aula")]
        public async Task<IActionResult> RealizarAula([FromBody] AulaAssistidaViewModel aulaAssistidaViewModel)
        {
            try
            {
                var ResultLogin = await _autenticacao.Login(aulaAssistidaViewModel.Login);

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

                            var conteudo = await _alunoService.RealizarAula(aulaAssistidaViewModel, accessToken);
                        }
                        else
                        {
                            return GenerateResponse(ResultLogin, ResponseTypeEnum.GenericError, HttpStatusCode.Unauthorized);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return CustomResponse();
        }


        [HttpPost("concluir-curso")]
        public async Task<IActionResult> ConcluirCurso([FromBody] ConcluirCursoViewModel concluirCursoViewModel)
        {
            try
            {
                var ResultLogin = await _autenticacao.Login(concluirCursoViewModel.Login);

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

                            var conteudo = await _alunoService.ConcluirCurso(concluirCursoViewModel, accessToken);
                        }
                        else
                        {
                            return GenerateResponse(ResultLogin, ResponseTypeEnum.GenericError, HttpStatusCode.Unauthorized);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return CustomResponse();
        }

    }
}