using MBA.Bff.Api.Services.Interface;
using MBA.WebApi.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Controllers
{
    [Authorize]
    public class ConteudoController(IConteudoService conteudoService) : MainController
    {
        private readonly IConteudoService _conteudoService = conteudoService;

        [HttpGet]
        [Route("Conteudo/Exemple")]
        public async Task<IActionResult> Index()
        {
            //return CustomResponse(await Exemple.ObterExemple());
            return CustomResponse();
        }
    }
}