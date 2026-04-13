using MBA.Conteudo.Api.ViewModels;
using MBA.Conteudo.Application.Services;
using MBA.Conteudo.Application.ViewModels;
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

namespace MBA.Conteudo.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CursoController(
	ICursoAppService cursoAppService,
	IAppIdentityUser appIdentityUser,
	INotificationHandler<DomainNotificacaoRaiz> notifications,
	IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
{
	[ClaimsAuthorize("Cursos", "AD")]
	[HttpPost]
	public async Task<IActionResult> CadastrarCurso([FromBody] CadastroCursoViewModel cadastroCursoViewModel)
	{
		if (!ModelState.IsValid)
			return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState);

		try
		{
			CadastroCursoDto dto = cadastroCursoViewModel;
			var cursoId = await cursoAppService.CadastrarCursoAsync(dto);
			return GenerateResponse(new { CursoId = cursoId }, ResponseTypeEnum.Success, HttpStatusCode.Created);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest,
				exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError,
				[ex.Message]);
		}
	}

	[ClaimsAuthorize("Cursos", "AT")]
	[HttpPut("{cursoId}")]
	public async Task<IActionResult> AtualizarCurso(Guid cursoId,
		[FromBody] AtualizacaoCursoViewModel atualizacaoCursoViewModel)
	{
		if (!ModelState.IsValid)
			return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState);
		if (cursoId != atualizacaoCursoViewModel.Id)
			return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden,
				["Você não tem permissão para realizar essa operação. Verifique sua requisição"]);

		try
		{
			AtualizacaoCursoDto dto = atualizacaoCursoViewModel;
			await cursoAppService.AtualizarCursoAsync(cursoId, dto);
			return GenerateResponse(null, ResponseTypeEnum.Success, HttpStatusCode.NoContent);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest,
				exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError,
				[ex.Message]);
		}
	}

	[ClaimsAuthorize("Cursos", "DS")]
	[HttpPatch("{cursoId}/desativar")]
	public async Task<IActionResult> DesativarCurso(Guid cursoId)
	{
		try
		{
			await cursoAppService.DesativarCursoAsync(cursoId);
			return GenerateResponse(null, ResponseTypeEnum.Success, HttpStatusCode.NoContent);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest,
				exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError,
				[ex.Message]);
		}
	}

	[ClaimsAuthorize("Cursos", "VI")]
	[HttpGet("{cursoId}")]
	public async Task<IActionResult> ObterPorId(Guid cursoId)
	{
		try
		{
			var curso = await cursoAppService.ObterPorIdAsync(cursoId);
			return GenerateResponse(curso);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.NotFound,
				exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError,
				[ex.Message]);
		}
	}

	[ClaimsAuthorize("Cursos", "VI")]
	[HttpGet("ativos")]
	public async Task<IActionResult> ObterAtivos()
	{
		try
		{
			var cursos = await cursoAppService.ObterAtivosAsync();
			return GenerateResponse(cursos);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError,
				[ex.Message]);
		}
	}

	[ClaimsAuthorize("Cursos", "VI")]
	[HttpGet]
	public async Task<IActionResult> ObterTodos()
	{
		try
		{
			var cursos = await cursoAppService.ObterTodosAsync();
			return GenerateResponse(cursos);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError,
				[ex.Message]);
		}
	}

	[AllowAnonymous]
	[HttpGet("{cursoId:guid}/aulas/total")]
	public async Task<IActionResult> ObterTotalAulas(Guid cursoId)
	{
		try
		{
			var total = await cursoAppService.ObterTotalAulasAsync(cursoId);
			return GenerateResponse(new { CursoId = cursoId, Total = total },
				ResponseTypeEnum.Success, HttpStatusCode.OK);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError,
				HttpStatusCode.NotFound, exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError,
				HttpStatusCode.InternalServerError, [ex.Message]);
		}
	}
}