using MBA.Auth.Api.Entidades;
using MBA.Auth.Api.ViewModels;
using MBA.Core.Messages.Integration;
using MBA.MessageBus;
using MBA.WebApi.Core.Identidade;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MBA.Auth.Api.Controllers;

[Route("api/identidade")]
public class AuthController(
	SignInManager<Usuarios> signInManager,
	UserManager<Usuarios> userManager,
	IOptions<AppSettings> appSettings,
	RoleManager<IdentityRole> roleManager,
	IMessageBus bus) : MainController
{
	private readonly AppSettings _appSettings = appSettings.Value;

	[HttpPost("nova-conta")]
	public async Task<ActionResult> Registrar(UsuarioRegistro usuarioRegistro)
	{
		if (!ModelState.IsValid) return CustomResponse(ModelState);

		var user = new Usuarios
		{
			UserName = usuarioRegistro.NomeUsuario,
			Email = usuarioRegistro.Email,
			EmailConfirmed = true,
			Administrador = usuarioRegistro.Administrador
		};

		var claimsToAdd = usuarioRegistro.Administrador
			? AdicionaClaimsAdmin()
			: AdicionaClaimsAluno();


		var result = await userManager.CreateAsync(user, usuarioRegistro.Senha);


		if (result.Succeeded)
		{
			var clienteResult = await RegistrarCliente(usuarioRegistro);
			var usuarioCriado = await userManager.FindByEmailAsync(usuarioRegistro.Email);

			if (!clienteResult.ValidationResult.IsValid)
			{
				await userManager.DeleteAsync(user);
				return CustomResponse(clienteResult.ValidationResult);
			}

			foreach (var claim in claimsToAdd) await userManager.AddClaimAsync(usuarioCriado!, claim);

			return CustomResponse(await GerarJwt(usuarioRegistro.Email));
		}

		foreach (var error in result.Errors) AdicionarErroProcessamento(error.Description);

		return CustomResponse();
	}


	private async Task<ResponseMessage> RegistrarCliente(UsuarioRegistro usuarioRegistro)
	{
		var usuario = await userManager.FindByEmailAsync(usuarioRegistro.Email);

		var usuarioRegistrado = new UsuarioRegistradoIntegrationEvent(
			Guid.Parse(usuario?.Id ?? Guid.NewGuid().ToString()), usuarioRegistro.NomeUsuario, usuarioRegistro.Email,
			usuarioRegistro.Administrador);

		try
		{
			return await bus.RequestAsync<UsuarioRegistradoIntegrationEvent, ResponseMessage>(usuarioRegistrado);
		}
		catch
		{
			await userManager.DeleteAsync(usuario!);
			throw;
		}
	}


	[HttpPost("autenticar")]
	public async Task<ActionResult> Login(UsuarioLogin usuarioLogin)
	{
		if (!ModelState.IsValid) return CustomResponse(ModelState);

		var user = await userManager.FindByEmailAsync(usuarioLogin.Email);
		if (user is null)
		{
			AdicionarErroProcessamento("Usuário ou Senha Incorretos");
			return CustomResponse();
		}

		var result = await signInManager.CheckPasswordSignInAsync(user, usuarioLogin.Senha, true);

		if (result.Succeeded) return CustomResponse(await GerarJwt(usuarioLogin.Email));

		if (result.IsLockedOut)
		{
			AdicionarErroProcessamento("Usuário temporariamente bloqueado por tentativas inválidas");
			return CustomResponse();
		}

		AdicionarErroProcessamento("Usuário ou Senha Incorretos");
		return CustomResponse();
	}

	private async Task<UsuarioRespostaLogin> GerarJwt(string email)
	{
		var user = await userManager.FindByEmailAsync(email);
		var claims = await userManager.GetClaimsAsync(user!);

		var identityClaims = await ObterClaimsUsuario(claims, user!);
		var encodedToken = CodificarToken(identityClaims);

		return ObterRespostaToken(encodedToken, user!, claims);
	}

	private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, Usuarios user)
	{
		var userRoles = await userManager.GetRolesAsync(user);

		claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
		claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""));
		claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
		claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
		claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(),
			ClaimValueTypes.Integer64));

		foreach (var userRole in userRoles) claims.Add(new Claim("role", userRole));

		// Include claims that are assigned to the roles (e.g. permission claims)
		foreach (var userRole in userRoles)
		{
			var identityRole = await roleManager.FindByNameAsync(userRole);
			if (identityRole == null) continue;

			var roleClaims = await roleManager.GetClaimsAsync(identityRole);
			foreach (var rc in roleClaims)
				// avoid duplicates
				if (!claims.Any(c => c.Type == rc.Type && c.Value == rc.Value))
					claims.Add(new Claim(rc.Type, rc.Value));
		}

		var identityClaims = new ClaimsIdentity();
		identityClaims.AddClaims(claims);

		return identityClaims;
	}

	private string CodificarToken(ClaimsIdentity identityClaims)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

		var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
		{
			Issuer = _appSettings.Emissor,
			Audience = _appSettings.ValidoEm,
			Subject = identityClaims,
			Expires = DateTime.UtcNow.AddHours(_appSettings.ExpiracaoHoras),
			SigningCredentials =
				new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		});
		return tokenHandler.WriteToken(token);
	}

	private UsuarioRespostaLogin ObterRespostaToken(string encodedToken, Usuarios user, IEnumerable<Claim> claims)
	{
		return new UsuarioRespostaLogin
		{
			AccessToken = encodedToken,
			ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
			UsuarioToken = new UsuarioToken
			{
				Id = user.Id,
				Email = user.Email!,
				Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
			}
		};
	}

	private static List<Claim> AdicionaClaimsAdmin()
	{
		var claims = new List<Claim>
		{
			new("Administrador", "ADM") // usuario com perfil de administrador
		};

		return claims;
	}

	private static List<Claim> AdicionaClaimsAluno()
	{
		var claimsToAdd = new[]
		{
			new Claim("Alunos", "Ler"), // matricular
			new Claim("Alunos", "RH"), // REGISTRAR HISTORICO
			new Claim("Alunos", "CC"), //CONCLUIR CURSO
			new Claim("Alunos", "SC"), //SOLICITAR CERTIFICADO
			new Claim("Alunos", "PG"), //PAGAMENTO
			new Claim("Alunos", "GT") //BUSCAR INFORMAÇÕES
		};
		return [.. claimsToAdd];
	}

	private static long ToUnixEpochDate(DateTime date)
	{
		return (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
			.TotalSeconds);
	}
}