namespace MBA.Auth.Api.ViewModels;

public class UsuarioRespostaLogin
{
	public string AccessToken { get; set; } = string.Empty;
	public double ExpiresIn { get; set; }
	public UsuarioToken UsuarioToken { get; set; } = null!;
}

public class UsuarioToken
{
	public string Id { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public IEnumerable<UsuarioClaim> Claims { get; set; } = [];
}

public class UsuarioClaim
{
	public string Value { get; set; } = string.Empty;
	public string Type { get; set; } = string.Empty;
}
