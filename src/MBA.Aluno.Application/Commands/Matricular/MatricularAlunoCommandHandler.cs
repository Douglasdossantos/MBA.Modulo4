using MBA.Aluno.Application.Services;
using MBA.Aluno.Domain.Entities;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.AlunoCommands;
using MBA.Core.SharedDto.Aluno.Enum;
using MediatR;

namespace MBA.Aluno.Application.Commands.Matricular;

public class MatricularAlunoCommandHandler : IRequestHandler<MatricularAlunoCommand, bool>
{
	private readonly IMatriculaRepository _matriculaRepository;
	private readonly IAlunoRepository _alunoRepository;
	private readonly IMediatorHandler _mediatorHandler;
	private readonly IConteudoService _conteudoService;

	private Guid _raizAgregacao;

	public MatricularAlunoCommandHandler(IMatriculaRepository matriculaRepository,
		IMediatorHandler mediatorHandler,
		IConteudoService conteudoService,
		IAlunoRepository alunoRepository)
	{
		_matriculaRepository = matriculaRepository;
		_mediatorHandler = mediatorHandler;
		_conteudoService = conteudoService;
		_alunoRepository = alunoRepository;
	}


	public async Task<bool> Handle(MatricularAlunoCommand request, CancellationToken cancellationToken)
	{
		_raizAgregacao = request.RaizAgregacao;
		if (!ValidarRequisicao(request)) return false;
		if (!await ObterAlunoAsync(request.AlunoId, cancellationToken)) return false;
		if (!await ValidarCursoAtivoAsync(request.CursoId, cancellationToken)) return false;
		if (!await ValidarAlunoJaMatriculado(request.AlunoId, request.CursoId)) return false;

		var matricula = new Matricula(request.CursoId, request.AlunoId, DateTime.Now,
			StatusMatricula.PendentePagamento);
		await _matriculaRepository.AdicionarAsync(matricula);

		return await _matriculaRepository.UnitOfWork.Commit();
	}

	private bool ValidarRequisicao(MatricularAlunoCommand request)
	{
		request.DefinirValidacao(new MatricularAlunoCommandValidator().Validate(request));
		if (!request.EhValido())
		{
			foreach (var erro in request.Erros)
				_mediatorHandler
					.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Domain.Entities.Aluno),
						erro)).GetAwaiter().GetResult();
			return false;
		}

		return true;
	}


	private async Task<bool> ObterAlunoAsync(Guid alunoId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var aluno = await _alunoRepository.ObterPorIdAsync(alunoId);
		if (aluno is null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await _mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, nameof(Aluno), "Aluno não encontrado.")
			);
			return false;
		}

		if (!aluno.Ativo)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await _mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, nameof(Aluno), "Aluno inativo não pode ser matriculado.")
			);
			return false;
		}

		return true;
	}

	private async Task<bool> ValidarCursoAtivoAsync(Guid cursoId, CancellationToken cancellationToken)
	{
		CursoDto? curso;
		try
		{
			curso = await _conteudoService.ObterCursoAsync(cursoId, cancellationToken);
		}
		catch (Exception ex)
		{
			await _mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, "Curso",
					$"Não foi possível validar o curso na Conteúdo API: {ex.Message}")
			);
			return false;
		}

		if (curso is null)
		{
			await _mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, "Curso", "Curso não encontrado.")
			);
			return false;
		}

		if (!curso.EstaDisponivel)
		{
			await _mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, "Curso", "Curso inativo ou indisponível.")
			);
			return false;
		}

		return true;
	}

	private async Task<bool> ValidarAlunoJaMatriculado(Guid alunoId, Guid cursoId)
	{
		var jaMatriculado = await _matriculaRepository.CheckAlunoJaMatriculado(alunoId, cursoId);
		if (jaMatriculado)
		{
			await _mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, "Aluno",
				"Aluno já matriculado neste curso"));
			return false;
		}

		return true;
	}
}