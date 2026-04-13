using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;

namespace MBA.WebApi.Core.Extensions;

public static class PollyExtensions
{
	public static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
	{
		var retry = HttpPolicyExtensions
			.HandleTransientHttpError()
			.WaitAndRetryAsync(
			[
				TimeSpan.FromSeconds(1),
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(10)
			]);

		return retry;
	}

	public static AsyncCircuitBreakerPolicy<HttpResponseMessage> CircuitBreaker()
	{
		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.CircuitBreakerAsync(
				handledEventsAllowedBeforeBreaking: 5,
				durationOfBreak: TimeSpan.FromSeconds(30));
	}
}
