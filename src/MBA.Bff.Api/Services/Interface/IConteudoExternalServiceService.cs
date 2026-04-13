using MBA.Bff.Api.Models.Conteudo;

using Refit;

namespace MBA.Bff.Api.Services.Interface;

public interface IConteudoExternalServiceService
{
	[Post("/api/Curso")]
	Task<HttpResponseMessage> CadastrarCurso([Body] CadastrarCursoRequest aulaViewModel,
		CancellationToken cancellationToken = default);
}
