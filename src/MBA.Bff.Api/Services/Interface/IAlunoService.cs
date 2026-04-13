using MBA.Bff.Api.Models.Aluno;
using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Services.Interface;

public interface IAlunoService
{
	Task<IActionResult> MatriculaPagamento(MatriculaViewModel matriculaViewModel, string authorization);
	Task<ContentResult> RealizarAula(AulaAssistidaViewModel aulaAssistidaViewModel, string authorization);
	Task<ContentResult> AlterarStatusMatricula(Guid matricula, int status, string authorization);
}