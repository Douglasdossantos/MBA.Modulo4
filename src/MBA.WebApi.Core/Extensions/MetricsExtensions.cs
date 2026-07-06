using Microsoft.AspNetCore.Builder;
using Prometheus;

namespace MBA.WebApi.Core.Extensions;

/// <summary>
/// Métricas Prometheus padronizadas para todos os serviços.
/// - Coleta métricas HTTP por request (contagem, duração, status) via <c>UseHttpMetrics</c>.
/// - Expõe o endpoint <c>/metrics</c> no formato de texto do Prometheus via <c>MapMetrics</c>.
/// Consumido por Prometheus/Grafana (scrape) no cluster. Chamar uma única vez no pipeline,
/// antes de <c>app.Run()</c>.
/// </summary>
public static class MetricsExtensions
{
    public static WebApplication UseDefaultMetrics(this WebApplication app)
    {
        app.UseHttpMetrics();
        app.MapMetrics();
        return app;
    }
}
