using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using System.Text;

namespace MBA.WebApi.Core.Identidade
{
    public static class JwtConfig
    {
        public static void AddJwtConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            // register JwtValidator for manual token validation when needed
            services.AddSingleton<IJwtValidator, JwtValidator>();       

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = appSettings.ValidoEm,
                    ValidIssuer = appSettings.Emissor
                };

                x.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Normaliza o token para evitar IDX14102 quando o client envia aspas/encoding no header.
                        var authHeader = context.Request.Headers.Authorization.ToString();
                        if (!string.IsNullOrWhiteSpace(authHeader))
                        {
                            var header = authHeader.Trim();

                            if (header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                                header = header["Bearer ".Length..].Trim();

                            // remove aspas externas caso o client envie: Authorization: Bearer "<token>"
                            if (header.Length >= 2 &&
                                ((header.StartsWith("\"") && header.EndsWith("\"")) ||
                                 (header.StartsWith("'") && header.EndsWith("'"))))
                            {
                                header = header.Substring(1, header.Length - 2).Trim();
                            }

                            // Se veio URL-encoded, tenta decodificar
                            try { header = System.Net.WebUtility.UrlDecode(header); } catch { }

                            context.Token = header;
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        try
                        {
                            var logger = context.HttpContext.RequestServices.GetService<ILoggerFactory>()?.CreateLogger("JwtBearer");
                            logger?.LogError(context.Exception, "JWT authentication failed: {Message}", context.Exception.Message);
                        }
                        catch { }

                        return Task.CompletedTask;
                    }
                };
            });
        }

        public static void UseAuthConfiguration(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();

        }
    }
}