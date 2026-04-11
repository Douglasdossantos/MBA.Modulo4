using MBA.Bff.Api.Models.Autenticacao;
using MBA.Bff.Api.Services.Interface;

using Microsoft.AspNetCore.Mvc;

namespace MBA.Bff.Api.Services.Implementation;

public class AutenticacaoService(IAutenticacaoExternalService autenticacaoService) : IAutenticacaoService
{
	public Task<IActionResult> Login(UsuarioLoginViewModel aulaViewModel)
	{
		return CallExternalLogin(aulaViewModel);
	}

	private async Task<IActionResult> CallExternalLogin(UsuarioLoginViewModel model)
	{
		var response = await autenticacaoService.Login(model);
		if (response == null)
			return new StatusCodeResult(StatusCodes.Status500InternalServerError);

		var content = await response.Content.ReadAsStringAsync();

		if (!response.IsSuccessStatusCode) return new ObjectResult(content) { StatusCode = (int)response.StatusCode };

		// try deserialize into expected DTO
		try
		{
			System.Text.Json.JsonSerializer.Deserialize<object>(content);
			return new ContentResult
			{
				Content = content,
				ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
				StatusCode = (int)response.StatusCode
			};
		}
		catch
		{
			return new ContentResult
			{
				Content = content,
				ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/json",
				StatusCode = (int)response.StatusCode
			};
		}
	}
}