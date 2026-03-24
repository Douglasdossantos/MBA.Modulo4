
using MBA.Conteudo.Data;

using Microsoft.EntityFrameworkCore;

namespace MBA.Conteudo.Api.MigrationHelp
{
    public static class DbMigrationHelper
    {
        private static ConteudoContext? _conteudoContext;

        public static async Task AutocarregamentoDadosAsync(WebApplication app)
        {
            var services = app.Services.CreateScope().ServiceProvider;
            await CarregamentoDadosAsync(services);
        }

        public static async Task CarregamentoDadosAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            _conteudoContext = scope.ServiceProvider.GetRequiredService<ConteudoContext>();

            if (env.IsDevelopment())
            {
                await _conteudoContext.Database.MigrateAsync();
                //_conteudoContext.Seed();
            }
        }
    }
}
