using MBA.Bff.Api.Models.Conteudo;
using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Services.Interface;

public interface IConteudoService
{
	Task<IActionResult> CadastrarCurso(CadastroCursoViewModel aulaViewModel, string authorization);
}