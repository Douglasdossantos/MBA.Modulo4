using MBA.Bff.Api.Models.Autenticacao;
using Refit;

namespace MBA.Bff.Api.Services.Interface
{
    public interface IAutenticacaoExternalService
    {
        [Post("/api/identidade/autenticar")]
        Task<HttpResponseMessage> Login([Body] UsuarioLoginViewModel usuarioLogin);
    }
}