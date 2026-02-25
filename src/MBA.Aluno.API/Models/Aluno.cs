using MBA.Core.DomainObjects;

namespace MBA.Aluno.API.Models
{
    public class Aluno : Entity, IAggregateRoot
    {
        public Aluno(Guid id, DateTime criadoEm)
        {
            Id = id;
            CriadoEm = criadoEm;
            Matriculas = new List<Matricula>();
            Excluido = false;
        }
        protected Aluno()  { }

        public DateTime CriadoEm { get; private set; }

        public ICollection<Matricula> Matriculas { get; private set; }
        public bool Excluido { get; private set; }

        public void CadastrarAluno(Guid id)
        {
            Id = id;
            CriadoEm = DateTime.Now;
        }
    }
}
