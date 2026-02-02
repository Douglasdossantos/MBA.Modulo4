using MBA.WebApp.MVC.Configuration;
using MBA.WebApp.MVC.Extensions;
using Microsoft.Extensions.Hosting;

namespace MBA.WebApp.MVC.Controllers
{
    public static class WebAppConfiguration
    {
        public static void AddMvcConfiguration(this IServiceCollection services,  IConfiguration configuration)
        {
            services.AddControllersWithViews();

            services.Configure<AppSettings>(
                configuration.GetSection("AppSettings"));
        }
        public static void UseMvcConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/erro/500");
                app.UseStatusCodePagesWithRedirects("/erro/{0}");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UserIdentityConfiguration();

            app.UseMiddleware<ExceptionMiddleware>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
