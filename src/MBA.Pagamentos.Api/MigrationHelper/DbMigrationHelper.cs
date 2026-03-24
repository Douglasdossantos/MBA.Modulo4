using MBA.Pagamentos.Data.Contexts;
using MBA.Pagamentos.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MBA.API.MigrationHelper
{
    public static class DbMigrationHelper
    {
        
        private static FaturamentoDbContext _faturamentoContext = null;

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

            _faturamentoContext = scope.ServiceProvider.GetRequiredService<FaturamentoDbContext>();

            if (env.IsDevelopment())
            {
                
                await _faturamentoContext.Database.MigrateAsync();
                //await PopularDatabaseAsync();
            }
        }

        //private static async Task PopularDatabaseAsync()
        //{
            

        //   // string roleAdminId = await CriarRegraAcessoAsync(_identityContext, "Administrador");
        //    //string roleUsuarioId = await CriarRegraAcessoAsync(_identityContext, "Usuario");

        //    await CriarUsuarioAsync("teste@gmail.com", "Password@2025", "teste filho", new DateTime(1999, 09, 08), roleAdminId, true);
        //    await CriarUsuarioAsync("antonio@gmail.com", "Password@2025", "antonio fabio", new DateTime(1998, 12, 31), roleUsuarioId, false);
        //    await CriarUsuarioAsync("maico@gmail.com", "Password@2025", "maico silva", new DateTime(2000, 06, 07), roleUsuarioId, false);
        //}

        //private static async Task CriarUsuarioAsync(string email, string senha, string nome, DateTime dataNascimento, string roleId, bool ehAdmin)
        //{
        //    var identityUser = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        //    var result = await _userManager.CreateAsync(identityUser, senha);

        //    if (result.Succeeded)
        //    {
        //        #region Roles
        //        //_identityContext.UserRoles.Add(new IdentityUserRole<string>()
        //        {
        //            RoleId = roleId,
        //            UserId = identityUser.Id.ToString()
        //        });

        //       // await _identityContext.SaveChangesAsync();
        //        #endregion Roles

        //        #region Data
        //        Guid userId = Guid.Parse(identityUser.Id);
        //        if (ehAdmin)
        //        {
        //            await CriarCursoAsync();
        //        }
        //        else
        //        {
        //            await CriarAlunoAsync(Guid.Parse(identityUser.Id), nome, email, dataNascimento);
        //        }
        //        #endregion
        //    }
        //}

        private static async  Task CriarPagamentoAsync(Guid alunoId, Guid cursoId, decimal valor)
        {
            //Pagamento pagamento = new Pagamento(alunoId, cursoId, valor);
            //_faturamentoContext.Pagamentos.Add(pagamento);
            //await _faturamentoContext.SaveChangesAsync();
        }

       

       
    }
}