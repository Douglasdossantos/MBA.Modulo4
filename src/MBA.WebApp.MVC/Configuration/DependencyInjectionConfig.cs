using MBA.WebApp.MVC.Extensions;
using MBA.WebApp.MVC.Services;

namespace MBA.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddHttpClient<IAutenticacaoService, AutenticacaoService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUser,AspNetUser>(); 
        }
    }
}
