namespace MBA.Conteudo.Api.Configuration
{
    public static class CorsConfiguration
    {
        public static IServiceCollection ConfigurarCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Dev", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());

                options.AddPolicy("Prod", builder =>
                    builder.WithOrigins("https://localhost:4000")
                        .WithMethods("OPTIONS", "GET", "POST", "PUT", "DELETE")
                        .AllowAnyHeader());
            });

            return services;
        }
    }
}
