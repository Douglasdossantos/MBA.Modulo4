using MBA.Core.SharedDto.Aluno.Enum;

namespace MBA.Core.Messages.AlunoCommands
{
    public class AlterarStatusMatriculaCommand : CommandRaiz
    {
        public Guid MatriculaId { get; private set; }
        public Enum Status { get; private set; }

        public AlterarStatusMatriculaCommand(Guid matriculaId, StatusMatricula status)
        {
            DefinirRaizAgregacao(matriculaId);
            MatriculaId = matriculaId;
            Status = status;
        }
    }
}
