using MBA.Aluno.Appplication.ViewModel;
using MBA.Core.SharedDto.Aluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Appplication.Interfaces
{
    public interface IAlunoAppService
    {
        Task<Guid> CadastrarAlunoAsync(AlunoViewModel dto);
        Task AtualizarAlunoAsync(Guid alunoId, AtualizarAlunoViewModel dto);
        Task<AlunoDto> ObterPorIdAsync(Guid alunoId);
        Task<AlunoDto> DesativarAlunoAsync(Guid alunoId);
        Task<AlunoDto> AtivarAlunoAsync(Guid alunoId);
    }
}
