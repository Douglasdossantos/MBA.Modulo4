using MBA.Auth.Api.Data;
using MBA.Auth.Api.Extensions;
using MBA.WebApi.Core.Identidade;
using Microsoft.AspNetCore.Identity;

namespace MBA.Auth.Api.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
        IConfiguration configuration)
        {

            services.AddIdentity<IdentityUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequireUppercase = true;
                o.Password.RequiredUniqueChars = 0;
                o.Password.RequiredLength = 8;
            }).AddErrorDescriber<IdentityMensagensPortugues>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddJwtConfiguration(configuration);

         

            //var appSettingsSection = configuration.GetSection("AppSettings");
            //services.Configure<AppSettings>(appSettingsSection);

            //var appSettings = appSettingsSection.Get<AppSettings>();
            //var key = Encoding.ASCII.GetBytes(appSettings.Secret);



            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(bearerOpitions =>
            //{
            //    bearerOpitions.RequireHttpsMetadata = true;
            //    bearerOpitions.SaveToken = true;
            //    bearerOpitions.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidAudience = appSettings.ValidoEm,
            //        ValidIssuer = appSettings.Emissor
            //    };
            //});

            return services;
        }
    }
}
