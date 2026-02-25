using MBA.Aluno.Api.MigrationHelp;
using MBA.WebApi.Core.Identidade;

namespace MBA.Aluno.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.RegistarServices();

            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
        {

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });

               DbMigrationHelper.AutocarregamentoDadosAsync(app).Wait();
            }
            app.UseCors("Total");

            app.UseHttpsRedirection();
            app.UseAuthConfiguration();

            app.MapControllers();

            return app;
        }
    }
}
