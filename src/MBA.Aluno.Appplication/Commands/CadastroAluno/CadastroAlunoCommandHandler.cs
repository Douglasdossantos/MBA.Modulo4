using FluentValidation.Results;

using MBA.Aluno.Domain.Interface;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using MBA.Core.Messages.AlunoCommands;

using MediatR;

namespace MBA.Aluno.Application.Commands.CadastroAluno;

public class CadastroAlunoCommandHandler : IRequestHandler<CadastroAlunoCommand, ValidationResult>
{
	private readonly IAlunoRepository _alunoRepository;
	private readonly IMediatorHandler _mediatorHandler;
	private Guid _raizAgregacao;

	public CadastroAlunoCommandHandler(IAlunoRepository alunoRepository, IMediatorHandler mediatorHandler)
	{
		_alunoRepository = alunoRepository;
		_mediatorHandler = mediatorHandler;
	}

	public async Task<ValidationResult> Handle(CadastroAlunoCommand request, CancellationToken cancellationToken)
	{
		_raizAgregacao = request.RaizAgregacao;

		if (!ValidarRequisicao(request))
			return request.ValidationResult;

		var aluno = new Domain.Entities.Aluno(
			request.AlunoId,
			request.Nome,
			request.Email,
			request.Ativo,
			request.Adm,
			request.DataCriacao);

		await _alunoRepository.AdicionarAsync(aluno);

		var sucesso = await _alunoRepository.UnitOfWork.Commit();

		if (!sucesso)
			request.DefinirValidacao(new ValidationResult(
				new List<ValidationFailure>
				{
					new("", "Erro ao salvar aluno")
				}));

		return request.ValidationResult;
	}

	private bool ValidarRequisicao(CadastroAlunoCommand request)
	{
		request.DefinirValidacao(new CadastroAlunoCommandValidator().Validate(request));
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