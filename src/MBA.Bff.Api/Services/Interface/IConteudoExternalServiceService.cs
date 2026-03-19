using MBA.Bff.Api.Models.Conteudo;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace MBA.Bff.Api.Services.Interface
{
    public interface IConteudoExternalServiceService
    {
        [Post("/api/Aula/{cursoId}")]
        Task<IActionResult> AdicionarAula(Guid cursoId, [Body] AdicionarAulaViewModel aulaViewModel);

        [Post("/api/Aula/{cursoId}")]
        Task<IActionResult> AtualizarAula(Guid cursoId, [Body] AtualizarAulaViewModel aulaViewModel);

        [Post("/api/Aula/{cursoId}/remover/{aulaId}")]
        Task<IActionResult> RemoverAula(Guid cursoId, Guid aulaId);

        [Get("/api/Aula/curso/{cursoId}/aulas")]
        Task<IActionResult> ObterAulasPorCurso(Guid cursoId);

        [Get("/api/Aulas")]
        Task<System.Net.Http.HttpResponseMessage> ObterTodasAulas();

        [Get("/api/Aula/{aulaId}")]
        Task<System.Net.Http.HttpResponseMessage> ObterAulaPorId(Guid aulaId);
    }
}