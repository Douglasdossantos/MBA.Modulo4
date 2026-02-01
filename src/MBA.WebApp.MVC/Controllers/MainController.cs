using MBA.WebApp.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace MBA.WebApp.MVC.Controllers
{
    public class MainController : Controller
    {
        protected bool ResponsePossuiErros(ResponseResult resposta)
        {
            if (resposta != null && resposta.Errors.Messagens.Any())
            {
                foreach (var mensage in resposta.Errors.Messagens)
                {
                    ModelState.AddModelError(string.Empty, mensage);
                }
                return true;
            }
            return false;
        }
    }
}
