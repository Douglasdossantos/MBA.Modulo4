using MBA.Aluno.Domain.Interface;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.AlunoCommands;

using MediatR;

namespace MBA.Aluno.Application.Commands.AlterarStatusMatricula;

public class AlterarStatusMatriculaCommandHandler : IRequestHandler<AlterarStatusMatriculaCommand, bool>
{
	private readonly IMatriculaRepository _matriculaRepository;
	private readonly IMediatorHandler _mediatorHandler;
	private Guid _raizAgregacao;

	public AlterarStatusMatriculaCommandHandler(
		IMatriculaRepository matriculaRepository,
		IMediatorHandler mediatorHandler)
	{
		_matriculaRepository = matriculaRepository;
		_mediatorHandler = mediatorHandler;
	}

	public async Task<bool> Handle(AlterarStatusMatriculaCommand request, CancellationToken cancellationToken)
	{
		_raizAgregacao = request.RaizAgregacao;

		if (!ValidarRequisicao(request)) return false;
		await _matriculaRepository.AtualizarStatusAsync(request.MatriculaId, request.Status);

		return await _matriculaRepository.UnitOfWork.Commit();
	}

	private bool ValidarRequisicao(AlterarStatusMatriculaCommand request)
	{
		request.DefinirValidacao(new AlterarStatusMatriculaCommandValidator().Validate(request));
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
}