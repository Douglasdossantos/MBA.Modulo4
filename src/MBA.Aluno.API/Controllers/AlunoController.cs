using AutoMapper;
using MBA.Aluno.Appplication.Interfaces;
using MBA.Aluno.Appplication.ViewModel;
using MBA.Core.Autentications;
using MBA.Core.DomainObjects;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.AlunoCommands;
using MBA.Core.SharedDto.Aluno;
using MBA.WebApi.Core.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MBA.Aluno.API.Controllers
{
    [Route("api/[controller]")]
    public class AlunoController(
            IAlunoAppService alunoAppService,
            IAlunoQuery alunoQuery,
            IMapper mapper,
            IAppIdentityUser appIdentityUser,
            INotificationHandler<DomainNotificacaoRaiz> notifications,
            IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
    {
        private readonly IAlunoAppService _alunoAppService = alunoAppService;
        private readonly IAlunoQuery _alunoQuery = alunoQuery;
        private readonly IMapper _mapper = mapper;


        [HttpPost("matricular-aluno")]
        public async Task<IActionResult> MatricularAluno(MatriculaViewModel matriculaCursoViewModel)
        {
            if (!ModelState.IsValid) { return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState); }

            try
            {
                //if (UserId != matriculaCursoViewModel.AlunoId) { return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden, ["Você não tem permissão para realizar essa operação"]); }

                //CursoDto cursoDto = await _cursoAppService.ObterPorIdAsync(matriculaCursoViewModel.CursoId);
                var comando = new MatricularAlunoCommand(matriculaCursoViewModel.CursoId, matriculaCursoViewModel.AlunoId);
                var sucesso = await _mediatorHandler.EnviarComandoRaiz(comando);
                if (sucesso)
                {
                    return GenerateResponse(new { matriculaCursoViewModel.AlunoId, matriculaCursoViewModel.CursoId },
                        responseType: ResponseTypeEnum.Success,
                        statusCode: HttpStatusCode.Created);
                }

                return GenerateResponse(responseType: ResponseTypeEnum.GenericError, statusCode: HttpStatusCode.BadRequest);
            }
            catch (DomainException exDomain)
            {
                return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
            }
            catch (Exception ex)
            {
                return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, [ex.Message]);
            }
        }

        [HttpPost("registrar-aula-assistida")]
        public async Task<IActionResult> RegistrarAulaAssistida(AulaAssistidaViewModel aulaAssistidaCursoViewModel)
        {
            if (!ModelState.IsValid) { return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState); }

            try
            {
                if (UserId != aulaAssistidaCursoViewModel.AlunoId) { return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden, ["Você não tem permissão para realizar essa operação"]); }

                var comando = new RegistrarAulaAssistidaCommand(aulaAssistidaCursoViewModel.AlunoId, aulaAssistidaCursoViewModel.MatriculaId, aulaAssistidaCursoViewModel.AulaId);
                var sucesso = await _mediatorHandler.EnviarComandoRaiz(comando);

                if (sucesso)
                {
                    return GenerateResponse(new { aulaAssistidaCursoViewModel.AlunoId, aulaAssistidaCursoViewModel.MatriculaId, aulaAssistidaCursoViewModel.AulaId },
                        responseType: ResponseTypeEnum.Success,
                        statusCode: HttpStatusCode.Created);
                }

                return GenerateResponse(responseType: ResponseTypeEnum.GenericError, statusCode: HttpStatusCode.BadRequest);
            }
            catch (DomainException exDomain)
            {
                return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
            }
            catch (Exception ex)
            {
                return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, [ex.Message]);
            }

        }

        [HttpPut("concluir-curso")]
        public async Task<IActionResult> ConcluirCurso(ConcluirCursoViewModel concluirCursoViewModel)
        {
            if (!ModelState.IsValid) { return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState); }

            try
            {
                if (UserId != concluirCursoViewModel.AlunoId) { return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden, ["Você não tem permissão para realizar essa operação"]); }

                var comando = new ConcluirCursoCommand(concluirCursoViewModel.MatriculaId, concluirCursoViewModel.AlunoId);
                var sucesso = await _mediatorHandler.EnviarComandoRaiz(comando);

                if (sucesso)
                {
                    return GenerateResponse(new { concluirCursoViewModel.AlunoId, concluirCursoViewModel.MatriculaId },
                        responseType: ResponseTypeEnum.Success,
                        statusCode: HttpStatusCode.Created);
                }

                return GenerateResponse(responseType: ResponseTypeEnum.GenericError, statusCode: HttpStatusCode.BadRequest);
            }
            catch (DomainException exDomain)
            {
                return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
            }
            catch (Exception ex)
            {
                return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, [ex.Message]);
            }

        }

        [HttpPut("{alunoId}/desativar")]
        public async Task<IActionResult> DesativarAluno(Guid alunoId)
        {
            try
            {
                await _alunoAppService.DesativarAlunoAsync(alunoId);
                return GenerateResponse(new { mensagem = "Aluno Desativado" }, ResponseTypeEnum.Success, HttpStatusCode.OK);
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

        [HttpPut("{alunoId}/Ativar")]
        public async Task<IActionResult> AtivarAluno(Guid alunoId)
        {
            try
            {
                await _alunoAppService.AtivarAlunoAsync(alunoId);
                return GenerateResponse(new { mensagem = "Aluno Ativado" }, ResponseTypeEnum.Success, HttpStatusCode.OK);
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

        [HttpPut("{matriculaId}/{status}/status-matricula")]
        public async Task<IActionResult> AlterarStatusMatricula(Guid matriculaId, int status)
        {
            if (!ModelState.IsValid) { return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState); }

            try
            {
                //if (UserId != matriculaCursoViewModel.AlunoId) { return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden, ["Você não tem permissão para realizar essa operação"]); }
                var statusInt = (Core.SharedDto.Aluno.Enum.StatusMatricula)status;

                var comando = new AlterarStatusMatriculaCommand(matriculaId, statusInt);
                var sucesso = await _mediatorHandler.EnviarComandoRaiz(comando);
                if (sucesso)
                {
                    return GenerateResponse("matricula alterada",
                        responseType: ResponseTypeEnum.Success,
                        statusCode: HttpStatusCode.Created);
                }

                return GenerateResponse(responseType: ResponseTypeEnum.GenericError, statusCode: HttpStatusCode.BadRequest);
            }
            catch (DomainException exDomain)
            {
                return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest, exDomain);
            }
            catch (Exception ex)
            {
                return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, [ex.Message]);
            }
        }

        [HttpGet("{idMatricula}/evolucao-curso")]
        public async Task<IActionResult> ObterEvolucaoMatriculasCursoDoAlunoPorIdAsync(Guid idMatricula)
        {
            var evolucao = await _alunoQuery.EvolucaoCursoPorMatriculaAsync(idMatricula);
            if (evolucao == null) { return GenerateResponse(null, ResponseTypeEnum.NotFound, HttpStatusCode.NotFound); }


            return GenerateResponse(_mapper.Map<MatriculaDto>(evolucao));
        }

        [HttpGet("{alunoId}/PorId")]
        public async Task<IActionResult> ObterPorId(Guid alunoId)
        {
            try
            {
                var dto = await _alunoAppService.ObterPorIdAsync(alunoId);
                return GenerateResponse(dto, ResponseTypeEnum.Success, HttpStatusCode.OK);
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
    }
}
