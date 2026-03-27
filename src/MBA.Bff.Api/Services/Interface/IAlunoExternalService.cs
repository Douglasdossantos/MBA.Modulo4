using MBA.Bff.Api.Models.Aluno;
using Refit;

namespace MBA.Bff.Api.Services.Interface
{
    public interface IAlunoExternalService
    {
        [Post("/api/Aluno/matricular-aluno")]
        Task<HttpResponseMessage> MatricularAluno([Body] MatriculaRequest matriculaViewModel, [Header("Authorization")] string authorization = null);

        [Post("/api/Aluno/registrar-aula-assistida")]
        Task<HttpResponseMessage> RegistrarAulaAssistida([Body] AulaAssistidaRequest matriculaViewModel, [Header("Authorization")] string authorization = null);

        [Get("/api/Aluno/{alunoId}/PorId")]
        Task<AlunoViewModel> ObterPorId(Guid alunoId, [Header("Authorization")] string authorization = null);
    }
}