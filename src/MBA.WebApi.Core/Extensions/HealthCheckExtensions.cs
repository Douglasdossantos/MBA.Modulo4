using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MBA.WebApi.Core.Extensions;

/// <summary>
/// Health checks padronizados para todos os serviços.
/// - /health/live  : liveness (o processo está de pé). Usado pelo livenessProbe do K8s.
/// - /health/ready : readiness (dependências OK, ex.: banco). Usado pelo readinessProbe.
/// Cada serviço registra suas dependências encadeando no IHealthChecksBuilder retornado
/// e marcando-as com a tag <see cref="ReadyTag"/>.
/// </summary>
public static class HealthCheckExtensions
{
	public const string LiveTag = "live";
	public const string ReadyTag = "ready";

	public static IHealthChecksBuilder AddDefaultHealthChecks(this IServiceCollection services)
	{
		return services.AddHealthChecks()
			.AddCheck("self", () => HealthCheckResult.Healthy(), tags: [LiveTag]);
	}

	public static IEndpointRouteBuilder MapDefaultHealthChecks(this IEndpointRouteBuilder endpoints)
	{
		endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
		{
			Predicate = check => check.Tags.Contains(LiveTag),
			ResponseWriter = WriteResponse
		});

		endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
		{
			Predicate = check => check.Tags.Contains(ReadyTag),
			ResponseWriter = WriteResponse
		});

		return endpoints;
	}

	private static Task WriteResponse(HttpContext context, HealthReport report)
	{
		context.Response.ContentType = "application/json";

		var payload = JsonSerializer.Serialize(new
		{
			status = report.Status.ToString(),
			totalDurationMs = report.TotalDuration.TotalMilliseconds,
			checks = report.Entries.Select(entry => new
			{
				name = entry.Key,
				status = entry.Value.Status.ToString(),
				description = entry.Value.Description,
				error = entry.Value.Exception?.Message
			})
		});

		return context.Response.WriteAsync(payload);
	}
}
