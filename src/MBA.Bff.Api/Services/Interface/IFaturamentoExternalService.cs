using MBA.Bff.Api.Models.Pagamento;
using Refit;

namespace MBA.Bff.Api.Services.Interface
{
    public interface IFaturamentoExternalService
    {
        [Post("/api/Faturamento/{alunoId}/registrar-pagamento")]
        Task<HttpResponseMessage> RealizarPagamento(Guid alunoId, [Body] RealizarPagamentoRequest pagamentoViewModel, [Header("Authorization")] string authorization = null);
    }
}