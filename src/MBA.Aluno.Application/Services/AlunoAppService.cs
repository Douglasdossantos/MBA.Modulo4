using MBA.Aluno.Application.Interfaces;
using MBA.Aluno.Application.ViewModel;
using MBA.Aluno.Domain.Interface;
using MBA.Core.DomainObjects;
using MBA.Core.SharedDto.Aluno;

namespace MBA.Aluno.Application.Services;

public class AlunoAppService : IAlunoAppService
{
	private readonly IAlunoRepository _alunoRepository;

	public AlunoAppService(IAlunoRepository alunoRepository)
	{
		_alunoRepository = alunoRepository;
	}

	public async Task<Guid> CadastrarAlunoAsync(AlunoViewModel dto)
	{
		if (string.IsNullOrWhiteSpace(dto.Email)) throw new DomainException("Email não pode estar vazio");
		if (await _alunoRepository.ExisteEmailAsync(dto.Email)) throw new DomainException("Email já Existente");
		Domain.Entities.Aluno aluno = dto;
		aluno.CriarData();
		await _alunoRepository.AdicionarAsync(aluno);

		await _alunoRepository.UnitOfWork.Commit();
		return aluno.Id;
	}

	public async Task AtualizarAlunoAsync(Guid alunoId, AtualizarAlunoViewModel dto)
	{
		if (string.IsNullOrWhiteSpace(dto.Email)) throw new DomainException("Email não pode estar vazio");
		var temAluno = await _alunoRepository.ObterPorIdAsync(alunoId) ??
						throw new DomainException("Aluno não encontrado");
		var alunoEmail = await _alunoRepository.ObterPorEmailAsync(dto.Email);
		if (alunoEmail == null) throw new DomainException("Aluno não encontrado");
		if (!alunoEmail.Id.Equals(alunoId)) throw new DomainException("Esse Email pertence a outro aluno");

		Domain.Entities.Aluno aluno = dto;
		aluno.CriarDataDeixaAMesma(temAluno.DataCriacao);

		await _alunoRepository.AtualizarAsync(aluno);

		await _alunoRepository.UnitOfWork.Commit();
	}

	public async Task<AlunoDto> DesativarAlunoAsync(Guid alunoId)
	{
		var aluno = await _alunoRepository.ObterPorIdAsync(alunoId) ??
					throw new DomainException("Aluno não encontrado");

		aluno.Desativar();
		await _alunoRepository.AtualizarAsync(aluno);
		await _alunoRepository.UnitOfWork.Commit();

		return aluno;
	}

	public async Task<AlunoDto> AtivarAlunoAsync(Guid alunoId)
	{
		var aluno = await _alunoRepository.ObterPorIdAsync(alunoId) ??
					throw new DomainException("Curso não encontrado");

		aluno.Ativar();
		await _alunoRepository.AtualizarAsync(aluno);
		await _alunoRepository.UnitOfWork.Commit();

		return aluno;
	}

	public async Task<AlunoDto> ObterPorIdAsync(Guid alunoId)
	{
		var aluno = await _alunoRepository.ObterComMatriculasAsync(alunoId)
					?? throw new DomainException("Aluno não encontrado");

		return aluno;
	}
}