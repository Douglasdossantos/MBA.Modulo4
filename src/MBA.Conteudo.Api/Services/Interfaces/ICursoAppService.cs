using MBA.Conteudo.Api.ViewModels;

namespace MBA.Conteudo.Api.Services.Interfaces
{
    public interface ICursoAppService
    {
        Task<Guid> CadastrarCursoAsync(CursoViewModel viewModel);
        Task AtualizarCursoAsync(Guid cursoId, AtualizacaoCursoViewModel viewModel);
        Task DesativarCursoAsync(Guid cursoId);
        Task<CursoViewModel> ObterPorIdAsync(Guid cursoId);
        Task<IEnumerable<CursoViewModel>> ObterAtivosAsync();
        Task<IEnumerable<CursoViewModel>> ObterTodosAsync();
        Task<ConteudoProgramaticoViewModel> ObterConteudoProgramaticoAsync(Guid cursoId);
    }
}
