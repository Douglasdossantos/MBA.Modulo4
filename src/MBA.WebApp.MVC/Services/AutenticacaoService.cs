using MBA.WebApp.MVC.Models;

namespace MBA.WebApp.MVC.Services;

public class AutenticacaoService : Service, IAutenticacaoService
{
	private readonly HttpClient _httpClient;

	public AutenticacaoService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<UsuarioRespostaLogin> Login(UsuarioLoginViewModel loginViewModel)
	{
		var loginContent = ObterConteudo(loginViewModel);
		var response = await _httpClient.PostAsync("/api/identidade/autenticar", loginContent);

		if (!TratarErrrosResponse(response))
			return new UsuarioRespostaLogin
			{
				ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
			};

		return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
	}

	public async Task<UsuarioRespostaLogin> Registro(UsuarioRegistroViewModel registroViewModel)
	{
		var registroConteudo = ObterConteudo(registroViewModel);
		var response = await _httpClient.PostAsync("/api/identidade/nova-conta", registroConteudo);

		if (!TratarErrrosResponse(response))
			return new UsuarioRespostaLogin
			{
				ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
			};

		return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
	}
}