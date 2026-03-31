using MBA.Core.SharedDto.Aluno.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Core.Messages.Integration
{
    public class AlterarStatusMatriculaIntegrationEvent : IntegrationEvent
    {
        public Guid Id { get; private set; }
        public StatusMatricula Status { get; private set; }

        public AlterarStatusMatriculaIntegrationEvent(Guid id, StatusMatricula status)
        {
            Id = id;
            Status = status;
        }
    }
}
