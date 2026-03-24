namespace MBA.Core.Messages.AlunoCommands
{
    public class RegistrarAlunoCommand : Command
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public bool Administrator { get; private set; }

        public RegistrarAlunoCommand(Guid id, string nome, string email, bool administrator)
        {
            AggregateId = id;
            Id = id;
            Nome = nome;
            Email = email;
            Administrator = administrator;
        }
    }
}
