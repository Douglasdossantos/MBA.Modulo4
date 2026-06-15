using MBA.Aluno.Application.ViewModel;
using MBA.Core.SharedDto.Aluno;

namespace MBA.Aluno.Application.Interfaces;

public interface IAlunoAppService
{
	Task<Guid> CadastrarAlunoAsync(AlunoViewModel dto);
	Task AtualizarAlunoAsync(Guid alunoId, AtualizarAlunoViewModel dto);
	Task<AlunoDto> ObterPorIdAsync(Guid alunoId);
	Task<AlunoDto> DesativarAlunoAsync(Guid alunoId);
	Task<AlunoDto> AtivarAlunoAsync(Guid alunoId);
}