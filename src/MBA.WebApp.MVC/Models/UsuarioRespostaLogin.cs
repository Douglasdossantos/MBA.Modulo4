namespace MBA.WebApp.MVC.Models;

public class UsuarioRespostaLogin
{
	public string AccessToken { get; set; } = string.Empty;
	public double ExpiresIn { get; set; }
	public UsuarioToken UsuarioToken { get; set; } = null!;
	public ResponseResult? ResponseResult { get; set; }
}
