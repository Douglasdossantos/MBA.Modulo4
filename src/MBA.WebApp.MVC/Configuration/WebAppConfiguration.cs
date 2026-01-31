namespace MBA.WebApp.MVC.Controllers
{
    public static class WebAppConfiguration
    {
        public static void AddMvcConfiguration(this IServiceCollection services)
        {
            services.AddControllersWithViews();
        }
        public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        { }
    }
}
