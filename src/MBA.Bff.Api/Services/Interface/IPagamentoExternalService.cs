using MBA.Bff.Api.Models.Pagamento;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace MBA.Bff.Api.Services.Interface
{
    public interface IPagamentoExternalService
    {
        [Post("/api/Faturamento/{alunoId}/registrar-pagamento")]
        Task<IActionResult> CreatePaymentIntent(Guid alunoId, [Body] AdicionarAulaViewModel pagamentoViewModel);
    }
}