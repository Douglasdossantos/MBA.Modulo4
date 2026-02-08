using MBA.Bff.Api.Extensions;
using MBA.Bff.Api.Services.Interface;
using Microsoft.Extensions.Options;

namespace MBA.Bff.Api.Services.Implementation
{
    public class PagamentoService : Service, IPagamentoService
    {
        private readonly HttpClient _httpClient;

        public PagamentoService(HttpClient httpClient, IOptions<AppServicesSettings> settings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.AlunoUrl);
        }


        //public async Task<ExemploDTO> Obter()
        //{
        //    var response = await _httpClient.GetAsync("/Exemplo/");

        //    TratarErrosResponse(response);

        //    return await DeserializarObjetoResponse<ExemploDTO>(response);
        //}
    }
}