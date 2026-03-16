using MBA.Aluno.Appplication.ViewModel;
using MBA.Core.SharedDto.Aluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Appplication.Interfaces
{
    public interface IMatriculaAppService
    {
        Task<Guid> CadastrarMatriculaAsync(MatriculaViewModel dto);

        Task<MatriculaDto> ObterPorIdAsync(Guid matriculaId);
    }
}
