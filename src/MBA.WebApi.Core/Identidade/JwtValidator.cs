using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MBA.WebApi.Core.Identidade;

public interface IJwtValidator
{
	ClaimsPrincipal ValidateToken(string token);
}

public class JwtValidator : IJwtValidator
{
	private readonly TokenValidationParameters _validationParameters;

	public JwtValidator(IOptions<AppSettings> optionsAccessor)
	{
		var appSettings = optionsAccessor.Value;
		var key = Encoding.ASCII.GetBytes(appSettings.Secret ?? string.Empty);

		_validationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(key),
			ValidateIssuer = true,
			ValidIssuer = appSettings.Emissor,
			ValidateAudience = true,
			ValidAudience = appSettings.ValidoEm,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};
	}

	public ClaimsPrincipal ValidateToken(string token)
	{
		if (string.IsNullOrWhiteSpace(token)) return null!;

		var handler = new JwtSecurityTokenHandler();
		try
		{
			var principal = handler.ValidateToken(token, _validationParameters, out var validatedToken);

			if (validatedToken is JwtSecurityToken jwt &&
				jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
				return principal;

			return null;
		}
		catch
		{
			return null;
		}
	}
}