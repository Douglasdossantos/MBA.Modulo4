using MBA.Aluno.API.Models.Enum;
using MBA.Core.DomainObjects;

namespace MBA.Aluno.API.Models
{
    public class Matricula : Entity
    {
        protected Matricula() { } 

        private Matricula(
            Guid alunoId,
            Guid cursoId,
            int codMatricula,
            StatusMatricula status)
        {
            Id = Guid.NewGuid();
            AlunoId = alunoId;
            CursoId = cursoId;
            CodMatricula = codMatricula;
            Status = status;
            CriadaEm = DateTime.UtcNow;
        }

        public Guid Id { get; private set; }
        public Guid AlunoId { get; private set; }
        public Guid CursoId { get; private set; }
        public int CodMatricula { get; private set; }
        public Aluno Aluno { get; private set; }

        public StatusMatricula Status { get; private set; }
        public DateTime CriadaEm { get; private set; }
        public DateTime? AtivadaEm { get; private set; }
        public DateTime? FinalizadaEm { get; private set; }

        public static Matricula Criar(
            Guid alunoId,
            Guid cursoId,
            int codMatricula,
            StatusMatricula status)
        {
            return new Matricula(alunoId, cursoId, codMatricula, status);
        }

        public void Ativar()
        {
            Status = StatusMatricula.Ativa;
            AtivadaEm = DateTime.UtcNow;
        }

        public void Finalizar()
        {
            Status = StatusMatricula.Concluida;
            FinalizadaEm = DateTime.UtcNow;
        }

    }
}
