using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Appplication.Interfaces
{
    public interface ICertificadoAppService
    {
        Task<Guid> CadastrarCertificadoAsync(Guid MatriculaId);
    }
}
