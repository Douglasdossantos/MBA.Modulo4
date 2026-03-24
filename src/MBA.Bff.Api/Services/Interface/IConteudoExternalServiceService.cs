using MBA.Bff.Api.Models.Conteudo;
using Refit;

namespace MBA.Bff.Api.Services.Interface
{
    public interface IConteudoExternalServiceService
    {
        [Post("/api/Curso")]
        Task<System.Net.Http.HttpResponseMessage> CadastrarCurso([Body] CadastrarCursoRequest aulaViewModel, [Header("Authorization")] string authorization = null);
    }
}