namespace MBA.Bff.Api.Response;

public class LoginResponse
{
	public string AccessToken { get; set; }
	public int ExpiresIn { get; set; }
	public UsuarioToken UsuarioToken { get; set; }
}

public class Claim
{
	public string Value { get; set; }
	public string Type { get; set; }
}

public class UsuarioToken
{
	public string Id { get; set; }
	public string Email { get; set; }
	public List<Claim> Claims { get; set; }
}