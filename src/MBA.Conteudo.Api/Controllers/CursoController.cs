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
	[Route("api/[controller]")]
	[ApiController]
	public class CursoController(
        ICursoAppService cursoAppService,
        IAppIdentityUser aspNetUser,
        INotificationHandler<DomainNotificacaoRaiz> notifications,
        IMediatorHandler mediatorHandler) : ConteudoMainController(aspNetUser, notifications, mediatorHandler)
	{
		private readonly ICursoAppService _cursoAppService = cursoAppService;

        //[ClaimsAuthorize("Cursos", "AD")]
        [HttpPost]
		public async Task<IActionResult> CadastrarCurso([FromBody] CadastroCursoViewModel cadastroCursoViewModel)
		{
			if (!ModelState.IsValid)
			{
				return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState);
			}

			try
			{
				var cursoId = await _cursoAppService.CadastrarCursoAsync(cadastroCursoViewModel);
				return GenerateResponse(new { CursoId = cursoId }, ResponseTypeEnum.Success, HttpStatusCode.Created);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(new object(), ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(new object(), ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, new List<string> { ex.Message });
			}
		}

		//[ClaimsAuthorize("Cursos", "AT")]
		[HttpPut("{cursoId}")]
		public async Task<IActionResult> AtualizarCurso(Guid cursoId, [FromBody] AtualizacaoCursoViewModel atualizacaoCursoViewModel)
		{
			if (!ModelState.IsValid)
			{
				return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState);
			}

			if (cursoId != atualizacaoCursoViewModel.Id)
			{
				return GenerateResponse(new object(), ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden,
					new List<string> { "Você não tem permissão para realizar essa operação. Verifique sua requisição" });
			}

			try
			{
				await _cursoAppService.AtualizarCursoAsync(cursoId, atualizacaoCursoViewModel);
				return GenerateResponse(new object(), ResponseTypeEnum.Success, HttpStatusCode.NoContent);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(new object(), ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(new object(), ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, new List<string> { ex.Message });
			}
		}

		//[ClaimsAuthorize("Cursos", "DS")]
		[HttpPatch("{cursoId}/desativar")]
		public async Task<IActionResult> DesativarCurso(Guid cursoId)
		{
			try
			{
				await _cursoAppService.DesativarCursoAsync(cursoId);
				return GenerateResponse(new object(), ResponseTypeEnum.Success, HttpStatusCode.NoContent);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(new object(), ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(new object(), ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, new List<string> { ex.Message });
			}
		}

		//[ClaimsAuthorize("Cursos", "VI")]
		[HttpGet("{cursoId}")]
		public async Task<IActionResult> ObterPorId(Guid cursoId)
		{
			try
			{
				var curso = await _cursoAppService.ObterPorIdAsync(cursoId);
				return GenerateResponse(curso, ResponseTypeEnum.Success, HttpStatusCode.OK);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(new object(), ResponseTypeEnum.DomainError, HttpStatusCode.NotFound, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(new object(), ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, new List<string> { ex.Message });
			}
		}

		//[ClaimsAuthorize("Cursos", "VI")]
		[HttpGet("ativos")]
		public async Task<IActionResult> ObterAtivos()
		{
			try
			{
				var cursos = await _cursoAppService.ObterAtivosAsync();
				return GenerateResponse(cursos, ResponseTypeEnum.Success, HttpStatusCode.OK);
			}
			catch (Exception ex)
			{
				return GenerateResponse(new object(), ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, [ex.Message]);
			}
		}

		//[ClaimsAuthorize("Cursos", "VI")]
		[HttpGet]
		public async Task<IActionResult> ObterTodos()
		{
			try
			{
				var cursos = await _cursoAppService.ObterTodosAsync();
				return GenerateResponse(cursos, ResponseTypeEnum.Success, HttpStatusCode.OK);
			}
			catch (Exception ex)
			{
				return GenerateResponse(new object(), ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, [ex.Message]);
			}
		}

		//[ClaimsAuthorize("Cursos", "VI")]
		[HttpGet("{cursoId}/conteudo-programatico")]
		public async Task<IActionResult> ObterConteudoProgramatico(Guid cursoId)
		{
			try
			{
				var conteudo = await _cursoAppService.ObterConteudoProgramaticoAsync(cursoId);
				return GenerateResponse(conteudo, ResponseTypeEnum.Success, HttpStatusCode.OK);
			}
			catch (DomainException exDomain)
			{
				return GenerateDomainExceptionResponse(new object(), ResponseTypeEnum.DomainError, HttpStatusCode.NotFound, exDomain);
			}
			catch (Exception ex)
			{
				return GenerateResponse(new object(), ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, [ex.Message]);
			}
		}
	}
}

