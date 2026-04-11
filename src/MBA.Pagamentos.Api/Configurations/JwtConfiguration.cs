using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace MBA.Pagamentos.Api.Configurations;

public static class JwtConfiguration
{
	public static IServiceCollection ConfigurarJwt(this IServiceCollection services, JwtSettings jwtSettings)
	{
		var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

		services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;

				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidIssuer = jwtSettings.Issuer,
					ValidAudience = jwtSettings.Audience,
					ClockSkew = TimeSpan.Zero
				};
			});


		return services;
	}
}