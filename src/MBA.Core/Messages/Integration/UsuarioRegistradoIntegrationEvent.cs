using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Core.Messages.Integration
{
    public class UsuarioRegistradoIntegrationEvent : IntegrationEvent
    {
        public UsuarioRegistradoIntegrationEvent(Guid id, DateTime criadoEm)
        {
            Id = id;
            CriadoEm = criadoEm;
        }

        public Guid Id { get; private set; }
        public DateTime CriadoEm { get; set; }
    }
}
