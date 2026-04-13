using MBA.WebApp.MVC.Models;

using Microsoft.AspNetCore.Mvc;

namespace MBA.WebApp.MVC.Controllers;

public class MainController : Controller
{
	protected bool ResponsePossuiErros(ResponseResult? resposta)
	{
		if (resposta?.Errors.Mensagens != null && resposta.Errors.Mensagens.Any())
		{
			foreach (var mensagem in resposta.Errors.Mensagens) ModelState.AddModelError(string.Empty, mensagem);
			return true;
		}

		return false;
	}
}