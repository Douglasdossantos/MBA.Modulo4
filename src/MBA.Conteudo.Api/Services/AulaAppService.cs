using AutoMapper;
using MBA.Conteudo.Api.Models.Interfaces;
using MBA.Conteudo.Api.Services.Interfaces;
using MBA.Conteudo.Api.ViewModels;
using MBA.Core.DomainObjects;

namespace MBA.Conteudo.Api.Services
{
    public class AulaAppService : IAulaAppService
    {
        private readonly IConteudoRepository _conteudoRepository;
        private readonly IMapper _mapper;

        public AulaAppService(IConteudoRepository conteudoRepository, IMapper mapper)
        {
            _conteudoRepository = conteudoRepository;
            _mapper = mapper;
        }

        public async Task<Guid> AdicionarAulaAsync(Guid cursoId, AdicionarAulaViewModel viewModel)
        {
            var curso = await _conteudoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado.");
            if (!curso.Ativo)
            {
                throw new DomainException("Não é possível adicionar aulas a um curso inativo.");
            }

            // Adicionar a aula ao curso
            curso.AdicionarAula(viewModel.Descricao, viewModel.CargaHoraria, viewModel.OrdemAula, viewModel.Url);

            await _conteudoRepository.AtualizarAsync(curso);
            await _conteudoRepository.UnitOfWork.Commit();

            // Retornar o ID da última aula adicionada
            var aulaAdicionada = curso.Aulas.OrderByDescending(a => a.OrdemAula).FirstOrDefault();
            return aulaAdicionada?.Id ?? Guid.Empty;
        }

        public async Task AtualizarAulaAsync(Guid cursoId, AtualizarAulaViewModel viewModel)
        {
            var curso = await _conteudoRepository.ObterPorIdAsync(cursoId);
            if (curso is null)
            {
                throw new DomainException("Curso não encontrado.");
            }

            var aula = curso.Aulas.FirstOrDefault(a => a.Id == viewModel.Id);
            if (aula is null)
            {
                throw new DomainException("Aula não encontrada neste curso.");
            }

            // Atualizar os dados da aula
            curso.AlterarDescricaoAula(viewModel.Id, viewModel.Descricao);
            curso.AlterarCargaHorariaAula(viewModel.Id, viewModel.CargaHoraria);
            curso.AlterarOrdemAula(viewModel.Id, viewModel.OrdemAula);
            curso.AlterarUrlAula(viewModel.Id, viewModel.Url);

            await _conteudoRepository.AtualizarAsync(curso);
            await _conteudoRepository.UnitOfWork.Commit();
        }

        public async Task RemoverAulaAsync(Guid cursoId, Guid aulaId)
        {
            var curso = await _conteudoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado.");
            var aula = curso.Aulas.FirstOrDefault(a => a.Id == aulaId);
            if (aula is null)
            {
                throw new DomainException("Aula não encontrada neste curso.");
            }

            curso.RemoverAula(aula);

            await _conteudoRepository.AtualizarAsync(curso);
            await _conteudoRepository.UnitOfWork.Commit();
        }

        public async Task<IEnumerable<AulaResultViewModel>> ObterAulasPorCursoAsync(Guid cursoId)
        {
            var curso = await _conteudoRepository.ObterPorIdAsync(cursoId);
            if (ReferenceEquals(curso, null))
            {
                throw new DomainException("Curso não encontrado.");
            }

            var aulasAtivas = curso.Aulas.Where(a => a.Ativo).OrderBy(a => a.OrdemAula);
            return _mapper.Map<IEnumerable<AulaResultViewModel>>(aulasAtivas);
        }

        public async Task<AulaResultViewModel> ObterAulaPorIdAsync(Guid aulaId)
        {
            var aula = await _conteudoRepository.ObterAulaPorIdAsync(aulaId) ?? throw new DomainException("Aula não encontrada.");
            if (!aula.Ativo)
            {
                throw new DomainException("Esta aula não está disponível.");
            }

            return _mapper.Map<AulaResultViewModel>(aula);
        }

        public async Task<IEnumerable<AulaResultViewModel>> ObterTodasAulasAsync()
        {
            var aulas = await _conteudoRepository.ObterTodasAulasAsync();
            return _mapper.Map<IEnumerable<AulaResultViewModel>>(aulas);
        }
    }
}
