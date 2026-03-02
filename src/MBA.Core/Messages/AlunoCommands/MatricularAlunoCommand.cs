using SaberOnline.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Core.Messages.AlunoCommands
{
    public class MatricularAlunoCommand : CommandRaiz
    {
        public Guid CursoId { get; private set; }
        public Guid AlunoId { get; private set; }

        public MatricularAlunoCommand(Guid cursoId, Guid alunoId)
        {
            CursoId = cursoId;
            AlunoId = alunoId;
        }


    }
}
