namespace MBA.Core.Authentications;

public interface IAppIdentityUser
{
	Guid ObterUsuarioId();
	bool EstahAutenticado();
	bool EhAdministrador();
	string ObterEmail();
}