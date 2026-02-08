using MBA.Bff.Api.Services.Interface;
using MBA.WebApi.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Controllers
{
    [Authorize]
    public class PagamentoController(IPagamentoService pagamentoService) : MainController
    {
        private readonly IPagamentoService _pagamentoService = pagamentoService;

        [HttpGet]
        [Route("Pagamento/Exemple")]
        public async Task<IActionResult> Index()
        {
            //return CustomResponse(await Exemple.ObterExemple());
            return CustomResponse();
        }
    }
}