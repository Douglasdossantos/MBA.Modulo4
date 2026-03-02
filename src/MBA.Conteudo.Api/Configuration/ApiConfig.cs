using MBA.Conteudo.Api.Data;
using MBA.Conteudo.Api.MigrationHelp;
using MBA.WebApi.Core.Identidade;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace MBA.Conteudo.Api.Configuration
{
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ConteudoContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
            });

            // JWT Authentication
            services.AddJwtConfiguration(configuration);


            //Será alterado mais a frente
            services.AddCors(options =>
            {
                options.AddPolicy("Total", builder =>

                    builder
                           .AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });
        }

        public static void UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                if (app is WebApplication webApp)
                {
                    // Executa migrations e seed automáticos apenas em ambiente de desenvolvimento
                    DbMigrationHelper.AutocarregamentoDadosAsync(webApp).Wait();
                }
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("Total");

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
