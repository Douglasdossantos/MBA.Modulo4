using MBA.Conteudo.Api.ViewModels;

namespace MBA.Conteudo.Api.Services.Interfaces
{
    public interface IAulaAppService
    {
        Task<Guid> AdicionarAulaAsync(Guid cursoId, AdicionarAulaViewModel viewModel);
        Task AtualizarAulaAsync(Guid cursoId, AtualizarAulaViewModel viewModel);
        Task RemoverAulaAsync(Guid cursoId, Guid aulaId);
        Task<IEnumerable<AulaResultViewModel>> ObterAulasPorCursoAsync(Guid cursoId);
        Task<AulaResultViewModel> ObterAulaPorIdAsync(Guid aulaId);
        Task<IEnumerable<AulaResultViewModel>> ObterTodasAulasAsync();
    }
}
