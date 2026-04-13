using MBA.Bff.Api.Models.Conteudo;
using MBA.Bff.Api.Services.Interface;

using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Services.Implementation;

public class ConteudoService(IConteudoExternalServiceService conteudoService) : IConteudoService
{
	public async Task<IActionResult> CadastrarCurso(CadastroCursoViewModel cadastroCursoViewModel, CancellationToken cancellationToken = default)
	{
		var request = new CadastrarCursoRequest
		{
			Nome = cadastroCursoViewModel.Nome,
			Valor = cadastroCursoViewModel.Valor,
			ValidoAte = cadastroCursoViewModel.ValidoAte,
			Finalidade = cadastroCursoViewModel.ConteudoProgramatico.Finalidade,
			Ementa = cadastroCursoViewModel.ConteudoProgramatico.Ementa
		};

		var response = await conteudoService.CadastrarCurso(request, cancellationToken);

		if (response is null)
			return new StatusCodeResult(StatusCodes.Status500InternalServerError);

		var content = await response.Content.ReadAsStringAsync(cancellationToken);

		return new ContentResult
		{
			Content = content,
			ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
			StatusCode = (int)response.StatusCode
		};
	}
}
