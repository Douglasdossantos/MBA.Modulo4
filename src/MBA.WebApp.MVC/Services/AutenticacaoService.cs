using MBA.WebApp.MVC.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MBA.WebApp.MVC.Services
{
    public class AutenticacaoService :Service, IAutenticacaoService
    {
        private readonly HttpClient _httpClient;

        public AutenticacaoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UsuarioRespostaLogin> Login(UsuarioLoginViewModel loginViewModel)
        {
            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginViewModel), Encoding.UTF8, "application/json" );
            var respose = await _httpClient.PostAsync("https://localhost:7163/api/identidade/autenticar", loginContent);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            if (!TratarErrrosResponse(respose))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = JsonSerializer.Deserialize<ResponseResult>(await respose.Content.ReadAsStringAsync(), options)
                };
            }

            return JsonSerializer.Deserialize<UsuarioRespostaLogin>(await respose.Content.ReadAsStringAsync(), options);

        }

        public async Task<UsuarioRespostaLogin> Registro(UsuarioRegistroViewModel registroViewModel)
        {
            var registroContent = new StringContent(
                JsonSerializer.Serialize(registroViewModel), Encoding.UTF8, "application/json");
            var respose = await _httpClient.PostAsync("https://localhost:7163/api/identidade/nova-conta", registroContent);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            if (!TratarErrrosResponse(respose))
            {
                return new UsuarioRespostaLogin
                {
                    ResponseResult = JsonSerializer.Deserialize<ResponseResult>(await respose.Content.ReadAsStringAsync(), options)
                };
            }

            return JsonSerializer.Deserialize<UsuarioRespostaLogin>(await respose.Content.ReadAsStringAsync(), options);
        }
    }
}
