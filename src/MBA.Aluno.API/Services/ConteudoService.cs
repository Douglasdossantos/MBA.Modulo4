using MBA.Aluno.Application.Services;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MBA.Aluno.API.Services;

/// <summary>
/// Consulta a Conteúdo API para validar cursos antes de operações críticas
/// (ex.: matrícula). O endpoint upstream é /api/Curso/{id} e devolve o payload
/// no formato { success, type, result: CursoViewModel }.
/// </summary>
public sealed class ConteudoService : IConteudoService
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	private readonly HttpClient _httpClient;
	private readonly ILogger<ConteudoService> _logger;

	public ConteudoService(HttpClient httpClient, ILogger<ConteudoService> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
	}

	public async Task<CursoDto?> ObterCursoAsync(Guid cursoId, CancellationToken cancellationToken)
	{
		var requestUri = $"api/Curso/{cursoId}";

		using var response = await _httpClient.GetAsync(requestUri, cancellationToken);

		if (response.StatusCode == HttpStatusCode.NotFound)
		{
			_logger.LogInformation("Curso {CursoId} não encontrado na Conteúdo API.", cursoId);
			return null;
		}

		if (!response.IsSuccessStatusCode)
		{
			var content = await response.Content.ReadAsStringAsync(cancellationToken);
			_logger.LogWarning(
				"Falha ao consultar curso {CursoId}. Status: {Status}. Body: {Body}",
				cursoId, (int)response.StatusCode, content);
			throw new HttpRequestException(
				$"Conteúdo API retornou {(int)response.StatusCode} ao consultar o curso.");
		}

		var envelope = await response.Content.ReadFromJsonAsync<ConteudoApiEnvelope<CursoDto>>(
			JsonOptions, cancellationToken);

		return envelope?.Result;
	}

	private sealed class ConteudoApiEnvelope<T>
	{
		public bool Success { get; set; }
		public string? Type { get; set; }
		public T? Result { get; set; }
	}
}
