using AutoMapper;
using MBA.Conteudo.Api.Models;
using MBA.Conteudo.Api.Models.Interfaces;
using MBA.Conteudo.Api.Models.ValueObjects;
using MBA.Conteudo.Api.Services.Interfaces;
using MBA.Conteudo.Api.ViewModels;
using MBA.Core.DomainObjects;

namespace MBA.Conteudo.Api.Services
{
    public class CursoAppService : ICursoAppService
    {
        private readonly IConteudoRepository _conteudoRepository;
        private readonly IMapper _mapper;

        public CursoAppService(IConteudoRepository conteudoRepository, IMapper mapper)
        {
            _conteudoRepository = conteudoRepository;
            _mapper = mapper;
        }

        public async Task<Guid> CadastrarCursoAsync(CadastroCursoViewModel viewModel)
        {
            // Verificar se já existe curso com mesmo nome
            if (await _conteudoRepository.ExisteCursoComMesmoNomeAsync(viewModel.Nome))
            {
                throw new DomainException("Já existe um curso com este nome.");
            }

            var conteudoProgramatico = new ConteudoProgramatico(
                viewModel.ConteudoProgramatico.Finalidade,
                viewModel.ConteudoProgramatico.Ementa
            );

            var curso = new Curso(
                viewModel.Nome,
                viewModel.Valor,
                viewModel.ValidoAte,
                conteudoProgramatico
            );

            await _conteudoRepository.AdicionarAsync(curso);
            await _conteudoRepository.UnitOfWork.Commit();

            return curso.Id;
        }

        public async Task AtualizarCursoAsync(Guid cursoId, AtualizacaoCursoViewModel viewModel)
        {
            var curso = await _conteudoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado.");

            // Verificar se já existe outro curso com mesmo nome
            var cursoMesmoNome = await _conteudoRepository.ExisteCursoComMesmoNomeAsync(viewModel.Nome);
            if (cursoMesmoNome && curso.Nome != viewModel.Nome)
            {
                throw new DomainException("Já existe outro curso com este nome.");
            }

            curso.AlterarNome(viewModel.Nome);
            curso.AlterarValor(viewModel.Valor);
            curso.AlterarValidadeCurso(viewModel.ValidoAte);
            curso.AtualizarConteudoProgramatico(
                viewModel.ConteudoProgramatico.Finalidade,
                viewModel.ConteudoProgramatico.Ementa
            );

            await _conteudoRepository.AtualizarAsync(curso);
            await _conteudoRepository.UnitOfWork.Commit();
        }

        public async Task DesativarCursoAsync(Guid cursoId)
        {
            var curso = await _conteudoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado.");
            await _conteudoRepository.DesativarAsync(curso);
            await _conteudoRepository.UnitOfWork.Commit();
        }

        public async Task<CursoViewModel> ObterPorIdAsync(Guid cursoId)
        {
            var curso = await _conteudoRepository.ObterPorIdAsync(cursoId);
            return curso is null ? throw new DomainException("Curso não encontrado.") : _mapper.Map<CursoViewModel>(curso);
        }

        public async Task<IEnumerable<CursoViewModel>> ObterAtivosAsync()
        {
            var cursos = await _conteudoRepository.ObterAtivosAsync();
            return _mapper.Map<IEnumerable<CursoViewModel>>(cursos);
        }

        public async Task<IEnumerable<CursoViewModel>> ObterTodosAsync()
        {
            var cursos = await _conteudoRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<CursoViewModel>>(cursos);
        }

        public async Task<ConteudoProgramaticoViewModel> ObterConteudoProgramaticoAsync(Guid cursoId)
        {
            var curso = await _conteudoRepository.ObterPorIdAsync(cursoId);
            return curso is null
                ? throw new DomainException("Curso não encontrado.")
                : _mapper.Map<ConteudoProgramaticoViewModel>(curso.ConteudoProgramatico);
        }
    }
}
