using MBA.Core.SharedDto.Aluno.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Core.SharedDto.Aluno
{
    public class MatriculaDto
    {
        public Guid Id { get; set; }
        public Guid CursoId { get; set; }
        public Guid AlunoId { get; set; }
        public DateTime DataMatricula { get; set; }
        public DateTime DataCursoConcluido { get; set; }
        public int TotalAulas { get; set; }
        public int AulasAssistidas { get; set; }
        public int AulasFaltantes { get; set; }
        public decimal Porcentagem { get; set; }

        public StatusMatricula Status { get; private set; }
        public CertificadoDto Certificado { get; set; }
    }
}
