using MBA.Bff.Api.Models.Conteudo;
using MBA.Bff.Api.Response;
using MBA.Bff.Api.Services.Interface;
using MBA.Core.Authentications;
using MBA.Core.DomainObjects;
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
public class AdminController(
	IAutenticacaoService autenticacao,
	IConteudoService conteudoService,
	IAppIdentityUser appIdentityUser,
	INotificationHandler<DomainNotificacaoRaiz> notifications,
	IMediatorHandler mediatorHandler) :
	MainController(appIdentityUser, notifications, mediatorHandler)
{
	[HttpPost("cadastro-de-curso")]
	public async Task<IActionResult> CadastroDeCurso([FromBody] CadastroCursoViewModel cadastroCurso)
	{
		try
		{
			var resultLogin = await autenticacao.Login(cadastroCurso.Login);

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

						var options = new JsonSerializerOptions
						{
							PropertyNameCaseInsensitive = true
						};

						var result = JsonSerializer.Deserialize<LoginResponse>(content, options);

						cadastroCurso.Login.AlunoId = Guid.Parse(result.UsuarioToken.Id);

						var conteudo = await conteudoService.CadastrarCurso(cadastroCurso, accessToken);

						return GenerateResponse(((ContentResult)conteudo).Content);
					}

					return GenerateResponse(resultLogin, ResponseTypeEnum.Unauthorized, HttpStatusCode.Unauthorized);
				}

				return GenerateResponse(resultLogin, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError);
			}

			return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.Unauthorized);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse("", ResponseTypeEnum.DomainError, HttpStatusCode.NotFound, exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse("", ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError,
				[ex.Message]);
		}
	}
}