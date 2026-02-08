using MBA.Core.DomainObjects;

namespace MBA.Aluno.API.Models
{
    public class Aluno : Entity
    {
        public DateTime CriadoEm { get; private set; }

        public ICollection<Matricula> Matriculas { get; private set; }
    }
}
