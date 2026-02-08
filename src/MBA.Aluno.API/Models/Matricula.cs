using MBA.Aluno.API.Models.Enum;
using MBA.Core.DomainObjects;

namespace MBA.Aluno.API.Models
{
    public class Matricula : Entity
    {
        public Guid Id { get; private set; }
        public Guid AlunoId { get; private set; }
        public Guid CursoId { get; private set; }

        public StatusMatricula Status { get; private set; }
        public DateTime CriadaEm { get; private set; }
        public DateTime? AtivadaEm { get; private set; }
        public DateTime? FinalizadaEm { get; private set; }
    }
}
