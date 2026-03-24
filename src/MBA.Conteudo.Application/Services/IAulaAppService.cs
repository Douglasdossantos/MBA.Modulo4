using MBA.Core.SharedDto;

namespace MBA.Conteudo.Application.Services;
public interface IAulaAppService 
{
    Task<Guid> AdicionarAulaAsync(Guid cursoId, AulaDto dto);
    Task AtualizarAulaAsync(Guid cursoId, AulaDto dto);
    Task RemoverAulaAsync(Guid cursoId, Guid aulaId);
}
