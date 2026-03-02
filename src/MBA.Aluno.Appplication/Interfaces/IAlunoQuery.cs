using MBA.Core.SharedDto.Aluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Appplication.Interfaces
{
    public interface IAlunoQuery
    {
        public Task<MatriculaDto> EvolucaoCursoPorMatriculaAsync(Guid matriculaId);
    }
}
