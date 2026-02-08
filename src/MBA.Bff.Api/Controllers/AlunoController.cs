using MBA.Bff.Api.Services.Interface;
using MBA.WebApi.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Controllers
{
    [Authorize]
    public class AlunoController(IAlunoService alunoService) : MainController
    {
        
        private readonly IAlunoService _alunoService = alunoService;

        [HttpGet]
        [Route("Aluno/Exemple")]
        public async Task<IActionResult> Index()
        {
            //return CustomResponse(await Exemple.ObterExemple());
            return CustomResponse();
        }
    }
}