using MBA.Bff.Api.Models.Conteudo;
using Refit;

namespace MBA.Bff.Api.Services.Interface
{
    public interface IConteudoExternalServiceService
    {
        [Post("/api/Curso")]
        Task<HttpResponseMessage> CadastrarCurso([Body] CadastrarCursoRequest aulaViewModel, [Header("Authorization")] string authorization = null);


        [Get("/api/Curso/{cursoId}")]
        Task<HttpResponseMessage> ObterCursoPorId(Guid cursoId, [Header("Authorization")] string authorization = null);
    }
}