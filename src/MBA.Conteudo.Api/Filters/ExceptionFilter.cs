using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MBA.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly IActionResultExecutor<ObjectResult> _executor;
        private readonly ILogger _logger;

        public ExceptionFilter(IActionResultExecutor<ObjectResult> executor, ILogger<ExceptionFilter> logger)
        {
            _executor = executor;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is Exception ex)
            {
                context.ExceptionHandled = true;
                _logger.LogError(context?.Exception ?? context.Exception, $"Ocorreu um erro inesperado: {context?.Exception?.Message ?? context.Exception?.ToString()}");

                ObjectResult output;
                var outputResponse = new
                {
                    success = false,
                    message = "Ops, aconteceu um erro inesperado",
                    internalMessage = context?.Exception?.Message ?? context.Exception?.ToString()
                };

                output = new ObjectResult(outputResponse)
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Value = outputResponse
                };

                _executor.ExecuteAsync(new ActionContext(context.HttpContext, context.RouteData, context.ActionDescriptor), output)
                    .GetAwaiter()
                    .GetResult();
            }
        }
    }
}
