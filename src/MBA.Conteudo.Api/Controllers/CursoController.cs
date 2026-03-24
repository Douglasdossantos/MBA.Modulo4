using AutoMapper;
using MBA.API.ViewModels;
using MBA.Conteudo.Application.Services;
using MBA.Conteudo.Application.ViewModels;
using MBA.Core.Autentications;
using MBA.Core.DomainObjects;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.WebApi.Core.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MBA.API.Controllers.Conteudo
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CursoController(ICursoAppService cursoAppService,
    IMapper mapper,
    IAppIdentityUser appIdentityUser,
    INotificationHandler<DomainNotificacaoRaiz> notifications,
    IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
    {
        private readonly ICursoAppService _cursoAppService = cursoAppService;
        private readonly IMapper _mapper = mapper;

        //[ClaimsAuthorize("Cursos", "AD")]
        [HttpPost]
        public async Task<IActionResult> CadastrarCurso([FromBody] CadastroCursoViewModel cadastroCursoViewModel)
        {
            if (!ModelState.IsValid) { return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState); }

            try
            {
                var dto = _mapper.Map<CadastroCursoDto>(cadastroCursoViewModel);
                var cursoId = await _cursoAppService.CadastrarCursoAsync(dto);
                return GenerateResponse(new { CursoId = cursoId }, ResponseTypeEnum.Success, HttpStatusCode.Created);
            }
            catch (DomainException exDomain)
            {
                return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
            }
            catch (Exception ex)
            {
                return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, [ex.Message]);
            }
        }

        //[ClaimsAuthorize("Cursos", "AT")]
        [HttpPut("{cursoId}")]
        public async Task<IActionResult> AtualizarCurso(Guid cursoId, [FromBody] AtualizacaoCursoViewModel atualizacaoCursoViewModel)
        {
            if (!ModelState.IsValid) { return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState); }
            if (cursoId != atualizacaoCursoViewModel.Id) { return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden, ["Você não tem permissão para realizar essa operação. Verifique sua requisição"]); }

            try
            {
                var dto = _mapper.Map<AtualizacaoCursoDto>(atualizacaoCursoViewModel);
                await _cursoAppService.AtualizarCursoAsync(cursoId, dto);
                return GenerateResponse(null, ResponseTypeEnum.Success, HttpStatusCode.NoContent);
            }
            catch (DomainException exDomain)
            {
                return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
            }
            catch (Exception ex)
            {
                return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, [ex.Message]);
            }
        }

       // [ClaimsAuthorize("Cursos", "DS")]
        [HttpPatch("{cursoId}/desativar")]
        public async Task<IActionResult> DesativarCurso(Guid cursoId)
        {
            try
            {
                await _cursoAppService.DesativarCursoAsync(cursoId);
                return GenerateResponse(null, ResponseTypeEnum.Success, HttpStatusCode.NoContent);
            }
            catch (DomainException exDomain)
            {
                return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
            }
            catch (Exception ex)
            {
                return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, [ex.Message]);
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
                return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.NotFound, exDomain);
            }
            catch (Exception ex)
            {
                return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, [ex.Message]);
            }
        }

       // [ClaimsAuthorize("Cursos", "VI")]
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
                return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, [ex.Message]);
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
                return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.InternalServerError, [ex.Message]);
            }
        }
    }
}
