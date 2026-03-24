using MBA.API.Filters;
using MBA.Core.Autentications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MBA.API.Configurations
{
    public static class ApiConfiguration
    {
        public static IServiceCollection ConfigurarApi(this IServiceCollection services)
        {
            services.AddScoped<IAppIdentityUser, AppIdentityUser>();

            services.AddControllers(options =>
            {
                options.Filters.Add<DomainExceptionFilter>();
                options.Filters.Add<ExceptionFilter>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            services.AddHsts(options =>
            {
                options.MaxAge = TimeSpan.FromDays(365);
                options.IncludeSubDomains = true;
                options.Preload = true;
            });

            return services;
        }
    }
}
