using MBA.Core.DomainObjects;

namespace MBA.Aluno.API.Models
{
    public class ProgressoAula: Entity
    {
        public ProgressoAula(Guid matriculaId, Guid aulaId, Matricula matricula, bool concluida, DateTime? concluidaEm) 
        {
            MatriculaId = matriculaId;
            AulaId = aulaId;
            Matricula = matricula;
            Concluida = concluida;
            ConcluidaEm = concluidaEm;
        }
        protected ProgressoAula() { }
        public Guid MatriculaId { get; private set; }
        public Guid AulaId { get; private set; }
        public Matricula Matricula { get; set; }
        public bool Concluida { get; private set; }
        public DateTime? ConcluidaEm { get; private set; }
    }
}
