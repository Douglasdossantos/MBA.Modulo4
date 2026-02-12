using MBA.Auth.Api.Entidades;
using MBA.Auth.Api.Extensions;
using MBA.Auth.Api.ViewModels;
using MBA.WebApi.Core.Identidade;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MBA.Auth.Api.Controllers
{
    
    [Route("api/identidade")]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(SignInManager<IdentityUser> signInManager,
                              UserManager<IdentityUser> userManager,
                              IOptions<AppSettings> appSettings,
                              RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _roleManager = roleManager;
        }

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar(UsuarioRegistro usuarioRegistro)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }

            var user = new Usuarios()
            {
                UserName = usuarioRegistro.NomeUsuario,
                Email = usuarioRegistro.Email,
                EmailConfirmed = true,
                Administrador = usuarioRegistro.Administrador
            };

            var claimsToAdd = new List<Claim>();

            if (usuarioRegistro.Administrador)
            {
                claimsToAdd = AdicionaClaimsAdmin();
            }
            else
            {
                claimsToAdd = AdicionaClaimsAluno();
            }


            var result = await _userManager.CreateAsync(user,usuarioRegistro.Senha);


            if (result.Succeeded)
            {
                var usuarioCriado = await _userManager.FindByEmailAsync(usuarioRegistro.Email);

                foreach (var claim in claimsToAdd)
                {
                    await _userManager.AddClaimAsync(usuarioCriado, claim);
                }

                return CustomResponse(await GerarJWT(usuarioRegistro.Email));
            }

            foreach (var error in result.Errors)
            {
                AdicionarErroProcessamento(error.Description);
            }

            return CustomResponse();
        }
        [HttpPost("autenticar")]
        public async Task<ActionResult> login(UsuarioLogin usuarioLogin)
        {
            if (!ModelState.IsValid)
            {
                return CustomResponse(ModelState);
            }
            var result =  await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, isPersistent: false, true);

            if (result.Succeeded)
            {
                return CustomResponse( await GerarJWT(usuarioLogin.Email));
            }

            if (result.IsLockedOut)
            {
                AdicionarErroProcessamento("usuário Temporareamente bloqueado por tentativas inválidas");
                return CustomResponse();
            }

            AdicionarErroProcessamento("Usuário ou Senha Incorretos");
            return CustomResponse();
        }
        private async Task<UsuarioRespostaLogin> GerarJWT(string email)
        {
            var  user = await _userManager.FindByNameAsync(email);
            var claims = await _userManager.GetClaimsAsync(user);

            var identityClaims = await ObterClaimsUsuario(claims, user);
            var encodedToken = CodificarToken(identityClaims);

            return ObterRespostaToken(encodedToken, user, claims);
        }
        private async Task<ClaimsIdentity> ObterClaimsUsuario(ICollection<Claim> claims, IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim("role",userRole));
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
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });
            return tokenHandler.WriteToken(token);
        }
        private UsuarioRespostaLogin ObterRespostaToken(string encodedToken, IdentityUser user, IEnumerable<Claim> claims)
        {
            return new UsuarioRespostaLogin
            {
                AccessToken = encodedToken,
                ExpiresIn = TimeSpan.FromHours(_appSettings.ExpiracaoHoras).TotalSeconds,
                UsuarioToken = new UsuarioToken
                {
                    Id = user.Id,
                    Email = user.Email,
                    Claims = claims.Select(c => new UsuarioClaim { Type = c.Type, Value = c.Value })
                }
            };
        }

        private List<Claim> AdicionaClaimsAdmin()
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("Administrador", "ADM"));// usuario com perfil de administrador

            return claims;
        }

        private List<Claim> AdicionaClaimsAluno()
        {
            var claimsToAdd = new[]
           {
                new Claim("Alunos", "Ler"), // matricular
                new Claim("Alunos", "RH"), // REGISTRAR HISTORICO
                new Claim("Alunos", "CC"), //CONCLUIR CURSO
                new Claim("Alunos", "SC"), //SOLICITAR CERTIFICADO
                new Claim("Alunos", "PG"), //PAGAMENTO
                new Claim("Alunos", "GT"), //BUSCAR INFORMAÇÕES
                
            };
            return claimsToAdd.ToList();
        }

        private static long ToUnixEpochDate(DateTime date)
            => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0,TimeSpan.Zero)).TotalSeconds);
    }
}
