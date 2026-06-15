using MBA.Aluno.Domain.Entities;
using MBA.Aluno.Domain.Interface;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.AlunoCommands;

using MediatR;

namespace MBA.Aluno.Application.Commands.RegistrarAulaAssistida;

public class RegistrarAulaAssistidaCommandHandler(
	IAulaAssistidaRepository aulaAssistidaRepository,
	IMediatorHandler mediatorHandler) : IRequestHandler<RegistrarAulaAssistidaCommand, bool>
{
	private Guid _raizAgregacao;

	public async Task<bool> Handle(RegistrarAulaAssistidaCommand request, CancellationToken cancellationToken)
	{
		_raizAgregacao = request.RaizAgregacao;

		if (!ValidarRequisicao(request)) return false;

		if (await aulaAssistidaRepository.CheckAulaJaAssistida(request.MatriculaCursoId, request.AulaId))
		{
			await mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, nameof(AulaAssistida),
					"Aula já registrada como assistida para esta matrícula."));
			return false;
		}

		var aulaAssistida = new AulaAssistida(request.MatriculaCursoId, request.AulaId, DateTime.Now);
		await aulaAssistidaRepository.AdicionarAsync(aulaAssistida);

		return await aulaAssistidaRepository.UnitOfWork.Commit();
	}

	private bool ValidarRequisicao(RegistrarAulaAssistidaCommand request)
	{
		request.DefinirValidacao(new RegistrarAulaAssistidaCommandValidator().Validate(request));
		if (request.EhValido()) return true;

		foreach (var erro in request.Erros)
		{
			mediatorHandler.PublicarNotificacaoDominio(
				new DomainNotificacaoRaiz(_raizAgregacao, nameof(Domain.Entities.Aluno), erro))
				.GetAwaiter().GetResult();
		}

		return false;
	}
}
