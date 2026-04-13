using MBA.Aluno.Application.Interfaces;
using MBA.Aluno.Application.ViewModel;
using MBA.Core.Authentications;
using MBA.Core.DomainObjects;
using MBA.Core.Enumerators;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.AlunoCommands;
using MBA.WebApi.Core.Controllers;
using MBA.WebApi.Core.Identidade;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace MBA.Aluno.API.Controllers;

[Route("api/[controller]")]
public class AlunoController( //ICursoAppService cursoAppService,
	IAlunoAppService alunoAppService,
	IAlunoQuery alunoQuery,
	IAppIdentityUser appIdentityUser,
	INotificationHandler<DomainNotificacaoRaiz> notifications,
	IMediatorHandler mediatorHandler) : MainController(appIdentityUser, notifications, mediatorHandler)
{
	//private readonly ICursoAppService _cursoAppService = cursoAppService;


	[HttpPost("matricular-aluno")]
	public async Task<IActionResult> MatricularAluno(MatriculaViewModel matriculaCursoViewModel)
	{
		if (!ModelState.IsValid)
			return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState);

		try
		{
			//if (UserId != matriculaCursoViewModel.AlunoId) { return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden, ["Você não tem permissão para realizar essa operação"]); }

			//CursoDto cursoDto = await _cursoAppService.ObterPorIdAsync(matriculaCursoViewModel.CursoId);
			var comando = new MatricularAlunoCommand(matriculaCursoViewModel.CursoId, matriculaCursoViewModel.AlunoId);
			var sucesso = await MediatorHandler.EnviarComandoRaiz(comando);
			if (sucesso)
				return GenerateResponse(new { matriculaCursoViewModel.AlunoId, matriculaCursoViewModel.CursoId },
					ResponseTypeEnum.Success,
					HttpStatusCode.Created);

			return GenerateResponse(responseType: ResponseTypeEnum.GenericError, statusCode: HttpStatusCode.BadRequest);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest,
				exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, [ex.Message]);
		}
	}


	[HttpPost("registrar-aula-assistida")]
	public async Task<IActionResult> RegistrarAulaAssistida(AulaAssistidaViewModel aulaAssistidaCursoViewModel)
	{
		if (!ModelState.IsValid)
			return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState);

		try
		{
			if (UserId != aulaAssistidaCursoViewModel.AlunoId)
				return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden,
					["Você não tem permissão para realizar essa operação"]);

			var comando = new RegistrarAulaAssistidaCommand(aulaAssistidaCursoViewModel.AlunoId,
				aulaAssistidaCursoViewModel.MatriculaId, aulaAssistidaCursoViewModel.AulaId);
			var sucesso = await MediatorHandler.EnviarComandoRaiz(comando);

			if (sucesso)
				return GenerateResponse(
					new
					{
						aulaAssistidaCursoViewModel.AlunoId,
						aulaAssistidaCursoViewModel.MatriculaId,
						aulaAssistidaCursoViewModel.AulaId
					},
					ResponseTypeEnum.Success,
					HttpStatusCode.Created);

			return GenerateResponse(responseType: ResponseTypeEnum.GenericError, statusCode: HttpStatusCode.BadRequest);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest,
				exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, [ex.Message]);
		}
	}

	[HttpPut("concluir-curso")]
	public async Task<IActionResult> ConcluirCurso(ConcluirCursoViewModel concluirCursoViewModel)
	{
		if (!ModelState.IsValid)
			return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState);

		try
		{
			if (UserId != concluirCursoViewModel.AlunoId)
				return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden,
					["Você não tem permissão para realizar essa operação"]);

			var comando = new ConcluirCursoCommand(concluirCursoViewModel.MatriculaId, concluirCursoViewModel.AlunoId);
			var sucesso = await MediatorHandler.EnviarComandoRaiz(comando);

			if (sucesso)
				return GenerateResponse(new { concluirCursoViewModel.AlunoId, concluirCursoViewModel.MatriculaId },
					ResponseTypeEnum.Success,
					HttpStatusCode.Created);

			return GenerateResponse(responseType: ResponseTypeEnum.GenericError, statusCode: HttpStatusCode.BadRequest);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest,
				exDomain);
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
			await alunoAppService.DesativarAlunoAsync(alunoId);
			return GenerateResponse(new { mensagem = "Aluno Desativado" });
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

	[HttpPut("{alunoId}/Ativar")]
	public async Task<IActionResult> AtivarAluno(Guid alunoId)
	{
		try
		{
			await alunoAppService.AtivarAlunoAsync(alunoId);
			return GenerateResponse(new { mensagem = "Aluno Ativado" });
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

	[HttpPut("{matriculaId}/{status}/status-matricula")]
	public async Task<IActionResult> AlterarStatusMatricula(Guid matriculaId, int status)
	{
		if (!ModelState.IsValid)
			return GenerateModelStateResponse(ResponseTypeEnum.ValidationError, HttpStatusCode.BadRequest, ModelState);

		try
		{
			//if (UserId != matriculaCursoViewModel.AlunoId) { return GenerateResponse(null, ResponseTypeEnum.ValidationError, HttpStatusCode.Forbidden, ["Você não tem permissão para realizar essa operação"]); }
			var statusInt = (Core.SharedDto.Aluno.Enum.StatusMatricula)status;

			var comando = new AlterarStatusMatriculaCommand(matriculaId, statusInt);
			var sucesso = await MediatorHandler.EnviarComandoRaiz(comando);
			if (sucesso)
				return GenerateResponse("matricula alterada",
					ResponseTypeEnum.Success,
					HttpStatusCode.Created);

			return GenerateResponse(responseType: ResponseTypeEnum.GenericError, statusCode: HttpStatusCode.BadRequest);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError, HttpStatusCode.BadRequest,
				exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError, HttpStatusCode.BadRequest, [ex.Message]);
		}
	}

	[HttpGet("{idMatricula}/evolucao-curso")]
	public async Task<IActionResult> ObterEvolucaoMatriculasCursoDoAlunoPorIdAsync(Guid idMatricula)
	{
		var evolucao = await alunoQuery.EvolucaoCursoPorMatriculaAsync(idMatricula);
		if (evolucao == null) return GenerateResponse(null, ResponseTypeEnum.NotFound, HttpStatusCode.NotFound);


		return GenerateResponse(evolucao);
	}

	[HttpGet("matricula/{matriculaId:guid}/status")]
	[ClaimsAuthorize("Administrador", "PG")]
	[ClaimsAuthorize("Alunos", "PG")]
	public async Task<IActionResult> ObterStatusMatricula(Guid matriculaId, CancellationToken cancellationToken)
	{
		try
		{
			var status = await alunoQuery.ObterStatusMatriculaAsync(matriculaId, cancellationToken);
			if (status is null)
				return GenerateResponse(null, ResponseTypeEnum.NotFound, HttpStatusCode.NotFound,
					["Matrícula não encontrada."]);

			return GenerateResponse(status, ResponseTypeEnum.Success, HttpStatusCode.OK);
		}
		catch (DomainException exDomain)
		{
			return GenerateDomainExceptionResponse(null, ResponseTypeEnum.DomainError,
				HttpStatusCode.BadRequest, exDomain);
		}
		catch (Exception ex)
		{
			return GenerateResponse(null, ResponseTypeEnum.GenericError,
				HttpStatusCode.InternalServerError, [ex.Message]);
		}
	}

	[HttpGet("{alunoId}/PorId")]
	public async Task<IActionResult> ObterPorId(Guid alunoId)
	{
		try
		{
			var dto = await alunoAppService.ObterPorIdAsync(alunoId);
			return GenerateResponse(dto);
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
}