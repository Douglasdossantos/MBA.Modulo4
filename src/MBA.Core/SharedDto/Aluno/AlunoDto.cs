using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Core.SharedDto.Aluno
{
    public class AlunoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public bool Adm { get; set; }

        public DateTime DataCriacao { get; set; }

        //public ICollection<MatriculaDto> Matriculas { get; set; }
    }
}
