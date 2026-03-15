using MBA.Conteudo.Api.Enumerators;
using MBA.Conteudo.Api.Services.Interfaces;
using MBA.Conteudo.Api.ViewModels;
using MBA.Core.Autentications;
using MBA.Core.DomainObjects;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MBA.Conteudo.Api.Controllers
{
	//[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class AulaController : ConteudoMainController
	{
		private readonly IAulaAppService _aulaAppService;

		public AulaController(
			IAulaAppService aulaAppService,
			IAppIdentityUser aspNetUser,
			INotificationHandler<DomainNotificacaoRaiz> notifications,
			IMediatorHandler mediatorHandler) : base(aspNetUser, notifications, mediatorHandler)
		{
			_aulaAppService = aulaAppService;
		}

		//[ClaimsAuthorize("Aulas", "AD")]
		[HttpPost("{cursoId}")]
		public async Task<IActionResult> AdicionarAula(Guid cursoId, [FromBody] AdicionarAulaViewModel aulaViewModel)
		{
			if (!ModelState.IsValid)
			{
				return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState);
			}

			if (cursoId != aulaViewModel.CursoId)
			{
				return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest,
					new List<string> { "O cursoId da rota deve ser igual ao cursoId enviado no corpo da requisição." });
			}

			try
			{
				var aulaId = await _aulaAppService.AdicionarAulaAsync(cursoId, aulaViewModel);
				return GenerateResponse(new { AulaId = aulaId }, ResponseTypeEnum.Success, HttpStatusCode.Created);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, new List<string> { ex.Message });
			}
		}

		// [ClaimsAuthorize("Aulas", "AT")]
		[HttpPut("{cursoId}")]
		public async Task<IActionResult> AtualizarAula(Guid cursoId, [FromBody] AtualizarAulaViewModel aulaViewModel)
		{
			if (!ModelState.IsValid)
			{
				return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState);
			}

			if (cursoId != aulaViewModel.CursoId)
			{
				return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest,
					new List<string> { "O cursoId da rota deve ser igual ao cursoId enviado no corpo da requisição." });
			}

			try
			{
				await _aulaAppService.AtualizarAulaAsync(cursoId, aulaViewModel);
				return GenerateResponse(null, ResponseTypeEnum.Success, HttpStatusCode.NoContent);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, new List<string> { ex.Message });
			}
		}

		//[ClaimsAuthorize("Aulas", "RM")]
		[HttpDelete("{cursoId}/remover/{aulaId}")]
		public async Task<IActionResult> RemoverAula(Guid cursoId, Guid aulaId)
		{
			try
			{
				await _aulaAppService.RemoverAulaAsync(cursoId, aulaId);
				return GenerateResponse(null, ResponseTypeEnum.Success, HttpStatusCode.NoContent);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, new List<string> { ex.Message });
			}
		}

		//[Authorize]
		[HttpGet("curso/{cursoId}/aulas")]
		public async Task<IActionResult> ObterAulasPorCurso(Guid cursoId)
		{
			try
			{
				var aulas = await _aulaAppService.ObterAulasPorCursoAsync(cursoId);
				return GenerateResponse(aulas, ResponseTypeEnum.Success, HttpStatusCode.OK);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.NotFound, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, new List<string> { ex.Message });
			}
		}

		// [Authorize]
		[HttpGet("/api/Aulas")]
		public async Task<IActionResult> ObterTodasAulas()
		{
			try
			{
				var aulas = await _aulaAppService.ObterTodasAulasAsync();
				return GenerateResponse(aulas, ResponseTypeEnum.Success, HttpStatusCode.OK);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.NotFound, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, new List<string> { ex.Message });
			}
		}

		//[Authorize]
		[HttpGet("{aulaId}")]
		public async Task<IActionResult> ObterAulaPorId(Guid aulaId)
		{
			try
			{
				var aula = await _aulaAppService.ObterAulaPorIdAsync(aulaId);
				return GenerateResponse(aula, ResponseTypeEnum.Success, HttpStatusCode.OK);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.NotFound, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, new List<string> { ex.Message });
			}
		}
	}
}

