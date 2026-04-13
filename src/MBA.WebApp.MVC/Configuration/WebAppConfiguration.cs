using MBA.WebApp.MVC.Extensions;

namespace MBA.WebApp.MVC.Configuration;

public static class WebAppConfiguration
{
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
			"default",
			"{controller=Home}/{action=Index}/{id?}");
	}
}