using MBA.WebApp.MVC.Models;

namespace MBA.WebApp.MVC.Services
{
    public interface IAutenticacaoService
    {
        Task<UsuarioRespostaLogin>Login(UsuarioLoginViewModel loginViewModel);
        Task<UsuarioRespostaLogin> Registro(UsuarioRegistroViewModel registroViewModel);
    }
}
