

using MBA.Conteudo.Domain.Entities;
using MBA.Conteudo.Domain.Interfaces;
using MBA.Core.DomainObjects;
using MBA.Core.SharedDto;


namespace MBA.Conteudo.Application.Services;
public class AulaAppService(IConteudoRepository cursoRepository) : IAulaAppService
{
    private readonly IConteudoRepository _cursoRepository = cursoRepository;

    public async Task<Guid> AdicionarAulaAsync(Guid cursoId, AulaDto dto)
    {
        var curso = await ObterCursoComAulaAsync(cursoId);

        curso.AdicionarAula(dto.Descricao, dto.CargaHoraria, dto.OrdemAula, dto.Url);
        var aulaAdicionada = curso.Aulas.Last();
        await _cursoRepository.AdicionarAulaAsync(aulaAdicionada);
        await _cursoRepository.UnitOfWork.Commit();

        return aulaAdicionada.Id;
    }

    public async Task AtualizarAulaAsync(Guid cursoId, AulaDto dto)
    {
        var curso = await ObterCursoComAulaAsync(cursoId);

        curso.AlterarDescricaoAula(dto.Id, dto.Descricao);
        curso.AlterarCargaHorariaAula(dto.Id, dto.CargaHoraria);
        curso.AlterarOrdemAula(dto.Id, dto.OrdemAula);
        curso.AlterarUrlAula(dto.Id, dto.Url);
        if (dto.Ativo) { curso.AtivarAula(dto.Id); }
        else { curso.DesativarAula(dto.Id); }

        await _cursoRepository.AtualizarAsync(curso);
        await _cursoRepository.UnitOfWork.Commit();
    }

    public async Task RemoverAulaAsync(Guid cursoId, Guid aulaId)
    {
        var curso = await ObterCursoComAulaAsync(cursoId);
        var aula = curso.ObterAulaPeloId(aulaId);

        curso.RemoverAula(aula);

        await _cursoRepository.AtualizarAsync(curso);
        await _cursoRepository.UnitOfWork.Commit();
    }

    #region Helpers
    private async Task<Curso> ObterCursoComAulaAsync(Guid cursoId) => await _cursoRepository.ObterPorIdAsync(cursoId) ?? throw new DomainException("Curso não encontrado");
    #endregion
}
