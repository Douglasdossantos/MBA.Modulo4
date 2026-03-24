using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MBA.Bff.Api.Configuration
{
    // Removes security requirements from operations marked with [AllowAnonymous]
    public class AllowAnonymousOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation == null || context == null) return;

            var hasAllowAnonymous = false;

            // Check method and declaring type for AllowAnonymous
            var methodInfo = context.MethodInfo;
            if (methodInfo != null)
            {
                hasAllowAnonymous = methodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
                    || methodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() == true;
            }

            if (hasAllowAnonymous)
            {
                // Remove any security requirements so Swagger UI won't show padlock/requirement for this operation
                operation.Security?.Clear();
            }
        }
    }
}
