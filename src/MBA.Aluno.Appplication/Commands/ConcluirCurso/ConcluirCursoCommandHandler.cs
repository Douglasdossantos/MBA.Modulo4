using MBA.Aluno.Application.Interfaces;
using MBA.Aluno.Domain.Entities;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.AlunoCommands;

using MediatR;

namespace MBA.Aluno.Application.Commands.ConcluirCurso;

public class ConcluirCursoCommandHandler : IRequestHandler<ConcluirCursoCommand, bool>
{
	private readonly IMatriculaRepository _matriculaRepository;
	private readonly IMediatorHandler _mediatorHandler;
	private readonly IAlunoQuery _alunoQuery;
	private Guid _raizAgregacao;

	public ConcluirCursoCommandHandler(IAlunoRepository alunoRepository,
		IMatriculaRepository matriculaRepository,
		IMediatorHandler mediatorHandler,
		IAlunoQuery alunoQuery)
	{
		_ = alunoRepository;
		_matriculaRepository = matriculaRepository;
		_mediatorHandler = mediatorHandler;
		_alunoQuery = alunoQuery;
	}


	public async Task<bool> Handle(ConcluirCursoCommand request, CancellationToken cancellationToken)
	{
		_raizAgregacao = request.RaizAgregacao;
		if (!ValidarRequisicao(request)) return false;
		if (!await ObterEvolucaoAsync(request.MatriculaId)) return false;
		if (!ObterMatricula(request.MatriculaId, out var matricula)) return false;

		matricula!.StatusConcluido();
		matricula.CriarDataConcluido();

		var certificado = new Certificado(request.MatriculaId);
		certificado.CriarData();
		certificado.Path();
		await _matriculaRepository.AtualizarAsync(matricula);
		await _matriculaRepository.AdicionarAsync(certificado);

		return await _matriculaRepository.UnitOfWork.Commit();
	}


	private bool ObterMatricula(Guid matriculaId, out Matricula? matricula)
	{
		matricula = _matriculaRepository.ObterPorIdAsync(matriculaId).Result;
		if (matricula is null)
		{
			_mediatorHandler
				.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Matricula),
					"Matricula não encontrado.")).GetAwaiter().GetResult();
			return false;
		}

		return true;
	}

	private bool ValidarRequisicao(ConcluirCursoCommand request)
	{
		request.DefinirValidacao(new ConcluirCursoCommandValidator().Validate(request));
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


	private async Task<bool> ObterEvolucaoAsync(Guid idMatricula)
	{
		var matricula = await _alunoQuery.EvolucaoCursoPorMatriculaAsync(idMatricula);
		if (matricula == null)
		{
			_mediatorHandler
				.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao, nameof(Domain.Entities.Aluno),
					"Matricula não encontrado.")).GetAwaiter().GetResult();
			return false;
		}

		if (matricula.AulasFaltantes != 0)
		{
			_mediatorHandler.PublicarNotificacaoDominio(new DomainNotificacaoRaiz(_raizAgregacao,
				nameof(Domain.Entities.Aluno), "Voce ainda tem Aulas a serem concluidas.")).GetAwaiter().GetResult();
			return false;
		}

		return true;
	}
}