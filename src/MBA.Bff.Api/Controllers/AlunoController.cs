using MBA.Bff.Api.Models.Aluno;
using MBA.Bff.Api.Response;
using MBA.Bff.Api.Services.Interface;
using MBA.Core.Authentications;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Controllers;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using System.Net;
using System.Text.Json;

namespace MBA.Bff.Api.Controllers;

[Route("api/[controller]")]
public class AlunoController(
	IAutenticacaoService autenticacao,
	IAlunoService alunoService,
	IAppIdentityUser appIdentityUser,
	INotificationHandler<DomainNotificacaoRaiz> notifications,
	IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
{
	[HttpPost("matricula-pagamento")]
	public async Task<IActionResult> MatriculaPagamento([FromBody] MatriculaViewModel matriculaViewModel)
	{
		var resultLogin = await autenticacao.Login(matriculaViewModel.Login);

		if (resultLogin != null)
		{
			string content = null;

			if (resultLogin is ContentResult cr)
			{
				content = cr.Content;
			}
			else if (resultLogin is ObjectResult orr)
			{
				if (orr.Value is string s) content = s;
				else content = JsonSerializer.Serialize(orr.Value);
			}
			else if (resultLogin is JsonResult jr)
			{
				content = JsonSerializer.Serialize(jr.Value);
			}

			if (!string.IsNullOrWhiteSpace(content))
			{
				using var doc = JsonDocument.Parse(content);
				if (doc.RootElement.TryGetProperty("accessToken", out var tokenProp))
				{
					var accessToken = tokenProp.GetString();
					JsonSerializerOptions options = new()
					{
						PropertyNameCaseInsensitive = true
					};

					var result = JsonSerializer.Deserialize<LoginResponse>(content, options);

					matriculaViewModel.Login.AlunoId = Guid.Parse(result.UsuarioToken.Id);


					// pass token to service if needed (example shows calling service after auth)
					var conteudo = await alunoService.MatriculaPagamento(matriculaViewModel, accessToken);
					return GenerateResponse(((ContentResult)conteudo).Content);
				}
				else
				{
					// accessToken not present - treat as auth failure
					return GenerateResponse(resultLogin, ResponseTypeEnum.GenericError,
						HttpStatusCode.Unauthorized);
				}
			}
		}

		return CustomResponse();
	}

	[HttpPost("realizar-aula")]
	public async Task<IActionResult> RealizarAula([FromBody] AulaAssistidaViewModel aulaAssistidaViewModel)
	{
		var resultLogin = await autenticacao.Login(aulaAssistidaViewModel.Login);

		if (resultLogin != null)
		{
			string content = null;

			if (resultLogin is ContentResult cr)
			{
				content = cr.Content;
			}
			else if (resultLogin is ObjectResult orr)
			{
				if (orr.Value is string s) content = s;
				else content = JsonSerializer.Serialize(orr.Value);
			}
			else if (resultLogin is JsonResult jr)
			{
				content = JsonSerializer.Serialize(jr.Value);
			}

			if (!string.IsNullOrWhiteSpace(content))
			{
				using var doc = JsonDocument.Parse(content);
				if (doc.RootElement.TryGetProperty("accessToken", out var tokenProp))
				{
					var accessToken = tokenProp.GetString();

					await alunoService.RealizarAula(aulaAssistidaViewModel, accessToken);
					//return GenerateResponse(((ContentResult)conteudo).Content, ResponseTypeEnum.Success, HttpStatusCode.OK);
				}
				else
				{
					// accessToken not present - treat as auth failure
					return GenerateResponse(resultLogin, ResponseTypeEnum.GenericError,
						HttpStatusCode.Unauthorized);
				}
			}
		}

		return CustomResponse();
	}
}