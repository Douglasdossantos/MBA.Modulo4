using MBA.WebApp.MVC.Configuration;

namespace MBA.WebApp.MVC.Controllers
{
    public static class WebAppConfiguration
    {
        public static void AddMvcConfiguration(this IServiceCollection services,  IConfiguration configuration)
        {
            services.AddControllersWithViews();
        }
        public static void UseMvcConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UserIdentityConfiguration();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
