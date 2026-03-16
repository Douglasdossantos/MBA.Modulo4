using SaberOnline.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Core.Messages.AlunoCommands
{
    public class RegistrarAulaAssistidaCommand : CommandRaiz
    {
        public Guid AlunoId { get; private set; }
        public Guid MatriculaCursoId { get; private set; }
        public Guid AulaId { get; private set; }

        public RegistrarAulaAssistidaCommand(Guid alunoId, Guid matriculaCursoId, Guid aulaId)
        {
            {
                DefinirRaizAgregacao(alunoId);

                AlunoId = alunoId;
                MatriculaCursoId = matriculaCursoId;
                AulaId = aulaId;
            }
        }
    }
}
