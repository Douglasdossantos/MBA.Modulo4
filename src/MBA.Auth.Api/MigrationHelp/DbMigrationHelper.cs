using MBA.Auth.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MBA.Auth.Api.MigrationHelp
{
    public static class DbMigrationHelper
    {
        private static ApplicationDbContext _identityContext = null;

        private static UserManager<IdentityUser> _userManager = null;

        public static async Task AutocarregamentoDadosAsync(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await CarregamentoDadosAsync(services);
        }

        public static async Task CarregamentoDadosAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            _identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (env.IsDevelopment())
            {
                await _identityContext.Database.MigrateAsync();
                await PopularDatabaseAsync();
            }
        }

        private static async Task PopularDatabaseAsync()
        {
            if (_identityContext.Roles.Any()) { return; }

            string roleAdminId = await CriarRegraAcessoAsync(_identityContext, "Administrador");
            string roleUsuarioId = await CriarRegraAcessoAsync(_identityContext, "Alunos");

            await CriarUsuarioAsync("adm@adm.com", "Adm@2026!", "SUPER USUARIO", new DateTime(1989, 09, 08), roleAdminId, true);
            await CriarUsuarioAsync("douglas@gmail.com", "Douglas@2026", "Douglas costa", new DateTime(1998, 12, 31), roleUsuarioId, false);
            await CriarUsuarioAsync("outro@gmail.com", "Senha@2026", "outro usuario", new DateTime(2000, 06, 07), roleUsuarioId, false);
        }

        private static async Task<string> CriarRegraAcessoAsync(ApplicationDbContext identityContext, string role)
        {
            string roleId = Guid.NewGuid().ToString();
            identityContext.Roles.Add(new IdentityRole
            {
                Id = roleId,
                Name = role,
                NormalizedName = role,
                ConcurrencyStamp = DateTime.Now.ToString()
            });

            await identityContext.SaveChangesAsync();

            return roleId;
        }

        private static async Task CriarUsuarioAsync(string email, string senha, string nome, DateTime dataNascimento, string roleId, bool ehAdmin)
        {
            var identityUser = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(identityUser, senha);

            if (result.Succeeded)
            {
                #region Roles
                _identityContext.UserRoles.Add(new IdentityUserRole<string>()
                {
                    RoleId = roleId,
                    UserId = identityUser.Id.ToString()
                });

                await _identityContext.SaveChangesAsync();
                #endregion Roles

                //#region Data
                //Guid userId = Guid.Parse(identityUser.Id);
                //if (ehAdmin)
                //{
                //    await CriarCursoAsync();
                //}
                //else
                //{
                //    await CriarAlunoAsync(Guid.Parse(identityUser.Id), nome, email, dataNascimento);
                //}
                //#endregion
            }
        }
    }
}
