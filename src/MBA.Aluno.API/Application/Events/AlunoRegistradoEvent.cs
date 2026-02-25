using MBA.Core.Messages;

namespace MBA.Aluno.API.Application.Events
{
    public class AlunoRegistradoEvent : Event
    {
        public AlunoRegistradoEvent(Guid id, DateTime criadoEm)
        {
            AggregateId = id;
            Id = id;
            CriadoEm = criadoEm;
        }

        public Guid Id { get; private set; }
        public DateTime CriadoEm { get; private set; }
    }
}
