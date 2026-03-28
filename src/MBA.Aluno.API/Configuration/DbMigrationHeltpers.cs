using MBA.Aluno.API.Models.Enum;
using MBA.Aluno.Data.Context;
using MBA.Aluno.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MBA.Aluno.API.Configuration
{
    public static class DbMigrationHelpers
    {
        public static async Task EnsureSeedData(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await EnsureSeedData(services);
        }
        public static async Task EnsureSeedData(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            var context = scope.ServiceProvider.GetRequiredService<AlunoDbContext>();

            if (env.IsDevelopment() || env.IsEnvironment("Docker"))
            {
                await context.Database.MigrateAsync();
                await EnsureSeedProducts(context);
            }
        }
         
        private static async Task EnsureSeedProducts(AlunoDbContext context)
        {
            if (!context.Alunos.Any())
            {
                var alunos = new List<Domain.Entities.Aluno>
                {
                    new Domain.Entities.Aluno(Guid.NewGuid(), "Douglas", "douglas@email.com", true, false, DateTime.Now),
                    new Domain.Entities.Aluno(Guid.NewGuid(), "Maria", "maria@email.com", true, false, DateTime.Now)
                };

                await context.Alunos.AddRangeAsync(alunos);
                await context.SaveChangesAsync();
            }

            if (!context.Matriculas.Any())
            {
                var aluno = context.Alunos.First();

                var matricula = new Matricula(
                    Guid.NewGuid(),
                    aluno.Id,
                    DateTime.Now,
                    Core.SharedDto.Aluno.Enum.StatusMatricula.PagamentoRealizado
                );

                await context.Matriculas.AddAsync(matricula);
                await context.SaveChangesAsync();
            }

            if (!context.AulaAssistidas.Any())
            {
                var matricula = context.Matriculas.First();

                var aulasAssistidas = new List<AulaAssistida>
                {
                    new AulaAssistida(matricula.Id, Guid.NewGuid(), DateTime.Now.AddDays(-3)),
                    new AulaAssistida(matricula.Id, Guid.NewGuid(), DateTime.Now.AddDays(-2)),
                    new AulaAssistida(matricula.Id, Guid.NewGuid(), DateTime.Now.AddDays(-1))
                };

                await context.AulaAssistidas.AddRangeAsync(aulasAssistidas);
                await context.SaveChangesAsync();
            }

            if (!context.Certificados.Any())
            {
                var matricula = context.Matriculas.First();

                var certificado = new Certificado(matricula.Id);
                certificado.CriarData();
                certificado.Path();

                await context.Certificados.AddAsync(certificado);
                await context.SaveChangesAsync();
            }
        }
    }
}