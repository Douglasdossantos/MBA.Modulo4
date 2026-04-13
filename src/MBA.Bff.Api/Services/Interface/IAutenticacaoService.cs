using MBA.Bff.Api.Models.Autenticacao;
using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Services.Interface;

public interface IAutenticacaoService
{
	Task<IActionResult> Login(UsuarioLoginViewModel aulaViewModel);
}