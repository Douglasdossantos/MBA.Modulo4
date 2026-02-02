using MBA.WebApp.MVC.Extensions;
using MBA.WebApp.MVC.Models;
using Microsoft.Extensions.Options;

namespace MBA.WebApp.MVC.Services
{
    public class AutenticacaoService :Service, IAutenticacaoService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _settings;

        public AutenticacaoService(HttpClient httpClient,
            IOptions<AppSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<UsuarioRespostaLogin> Login(UsuarioLoginViewModel loginViewModel)
        {
           var loginContent = ObterConteudo(loginViewModel);

            //var response = await _httpClient.PostAsync($"{_settings.AutenticacaoUrl}/api/identidade/autenticar", loginContent);
            var response = await _httpClient.PostAsync("https://localhost:7163/api/identidade/autenticar", loginContent);

            if (!TratarErrrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);

        }

        public async Task<UsuarioRespostaLogin> Registro(UsuarioRegistroViewModel registroViewModel)
        {
            var registroConteudo = ObterConteudo(registroViewModel);

           // var response = await _httpClient.PostAsync($"{_settings.AutenticacaoUrl}/api/identidade/nova-conta", registroConteudo);
            var response = await _httpClient.PostAsync("https://localhost:7163/api/identidade/nova-conta", registroConteudo);

            if (!TratarErrrosResponse(response))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = await DeserializarObjetoResponse<ResponseResult>(response)
                };
            }

            return await DeserializarObjetoResponse<UsuarioRespostaLogin>(response);
        }
    }
}
