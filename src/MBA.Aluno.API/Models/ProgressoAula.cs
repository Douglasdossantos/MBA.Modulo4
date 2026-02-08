using MBA.Core.DomainObjects;

namespace MBA.Aluno.API.Models
{
    public class ProgressoAula: Entity
    {
        public Guid MatriculaId { get; private set; }
        public Guid AulaId { get; private set; }

        public bool Concluida { get; private set; }
        public DateTime? ConcluidaEm { get; private set; }
    }
}
