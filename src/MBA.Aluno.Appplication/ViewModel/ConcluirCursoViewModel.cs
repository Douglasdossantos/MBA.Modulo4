using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Appplication.ViewModel
{
    public class ConcluirCursoViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid AlunoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid MatriculaId { get; set; }
    }
}
