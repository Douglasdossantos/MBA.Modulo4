namespace MBA.Core.Messages.Integration;

public class UsuarioRegistradoIntegrationEvent : IntegrationEvent
{
	public Guid Id { get; private set; }
	public string Nome { get; private set; }
	public string Email { get; private set; }
	public bool Administrador { get; private set; }

	public UsuarioRegistradoIntegrationEvent(Guid id, string nome, string email, bool administrador)
	{
		Id = id;
		Nome = nome;
		Email = email;
		Administrador = administrador;
	}
}