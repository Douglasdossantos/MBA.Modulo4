using MBA.Core.SharedDto.Aluno;
using MBA.Pagamentos.Application.Services;

using System.Net;
using System.Text.Json;

namespace MBA.Pagamentos.Api.Services;

public class AlunoService(HttpClient httpClient, ILogger<AlunoService> logger) : IAlunoService
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public async Task<MatriculaStatusDto?> ObterStatusMatriculaAsync(Guid matriculaId, CancellationToken cancellationToken)
	{
		var url = $"api/aluno/matricula/{matriculaId}/status";

		try
		{
			using var response = await httpClient.GetAsync(url, cancellationToken);

			if (response.StatusCode == HttpStatusCode.NotFound)
				return null;

			if (!response.IsSuccessStatusCode)
			{
				logger.LogWarning(
					"Aluno API retornou {StatusCode} ao consultar status da matrícula {MatriculaId}.",
					response.StatusCode, matriculaId);
				return null;
			}

			await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
			var envelope = await JsonSerializer.DeserializeAsync<AlunoApiEnvelope<MatriculaStatusDto>>(
				stream, JsonOptions, cancellationToken);

			return envelope?.Result;
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (Exception ex)
		{
			logger.LogError(ex,
				"Erro ao consultar status da matrícula {MatriculaId}.", matriculaId);
			return null;
		}
	}

	private sealed class AlunoApiEnvelope<T>
	{
		public bool Success { get; set; }
		public string? Type { get; set; }
		public T? Result { get; set; }
	}
}
