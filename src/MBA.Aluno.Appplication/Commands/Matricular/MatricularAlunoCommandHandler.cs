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

	//private readonly ICursoRepository _cursoRepository;
	private Guid _raizAgregacao;

	public MatricularAlunoCommandHandler(IMatriculaRepository matriculaRepository,
		IMediatorHandler mediatorHandler,
		//ICursoRepository cursoRepository,
		IAlunoRepository alunoRepository)
	{
		_matriculaRepository = matriculaRepository;
		_mediatorHandler = mediatorHandler;
		//_cursoRepository = cursoRepository;
		_alunoRepository = alunoRepository;
	}


	public async Task<bool> Handle(MatricularAlunoCommand request, CancellationToken cancellationToken)
	{
		_raizAgregacao = request.RaizAgregacao;
		if (!ValidarRequisicao(request)) return false;
		if (!ObterAluno(request.AlunoId)) return false;
		//if (!ObterCurso(request.CursoId, out Curso curso)) { return false; }
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


	private bool ObterAluno(Guid alunoId)
	{
		var aluno = _alunoRepository.ObterPorIdAsync(alunoId).Result;
		if (aluno is null)
		{
			_mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, nameof(Aluno), "Aluno não encontrado.")
			).GetAwaiter().GetResult();
			return false;
		}

		if (!aluno.Ativo)
		{
			_mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, nameof(Aluno), "Aluno inativo não pode ser matriculado.")
			).GetAwaiter().GetResult();
			return false; // <- interrompe fluxo
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

	/*private bool ObterCurso(Guid cursoId, out Curso curso)
	{
		curso = _cursoRepository.ObterPorIdAsync(cursoId).Result;
		if (curso == null)
		{
			_mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, nameof(Curso), "Curso não encontrado.")
			).GetAwaiter().GetResult();
			return false;
		}

		if (!curso.Ativo)
		{
			_mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, nameof(Curso), "Não pode matricular alunos em cursos inativos.")
			).GetAwaiter().GetResult();
			return false;
		}

		return true;
	}

	*/
}