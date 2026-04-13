using MBA.Bff.Api.Models.Conteudo;
using MBA.Bff.Api.Services.Interface;
using MBA.Core.Authentications;
using MBA.Core.DomainObjects;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Controllers;
using MBA.WebApi.Core.Identidade;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace MBA.Bff.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class AdminController(
	IConteudoService conteudoService,
	IAppIdentityUser appIdentityUser,
	INotificationHandler<DomainNotificacaoRaiz> notifications,
	IMediatorHandler mediatorHandler) :
	MainController(appIdentityUser, notifications, mediatorHandler)
{
	[HttpPost("cadastro-de-curso")]
	[ClaimsAuthorize("Cursos", "AD")]
	public async Task<IActionResult> CadastroDeCurso(
		[FromBody] CadastroCursoViewModel cadastroCurso,
		CancellationToken cancellationToken)
	{
		try
		{
			var conteudo = await conteudoService.CadastrarCurso(cadastroCurso, cancellationToken);

			if (conteudo is ContentResult cr)
				return GenerateResponse(cr.Content, ResponseTypeEnum.Success, HttpStatusCode.OK);

			return GenerateResponse(conteudo, ResponseTypeEnum.Success, HttpStatusCode.OK);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse("", ResponseTypeEnum.DomainError,
				HttpStatusCode.NotFound, exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse("", ResponseTypeEnum.GenericError,
				HttpStatusCode.InternalServerError, [ex.Message]);
		}
	}
}
