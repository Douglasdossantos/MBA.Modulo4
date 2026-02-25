using MBA.Aluno.API.Data;
using MBA.Aluno.API.Models;
using MBA.Aluno.API.Models.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MBA.Aluno.Api.MigrationHelp
{
    public static class DbMigrationHelper
    {
        private static AlunoContext _alunoContext = null;    
        

        public static async Task AutocarregamentoDadosAsync(WebApplication serviceScope)
        {
            var services = serviceScope.Services.CreateScope().ServiceProvider;
            await CarregamentoDadosAsync(services);
        }

        public static async Task CarregamentoDadosAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            _alunoContext = scope.ServiceProvider.GetRequiredService<AlunoContext>();

            if (env.IsDevelopment())
            {
                await _alunoContext.Database.MigrateAsync();
                await PopularDatabaseAsync();
            }
        }

        private static async Task PopularDatabaseAsync()
        {
            if (_alunoContext.Alunos.Any()) { return; }

            var  aluno = await CriarAlunotesteAsync(_alunoContext);

            await MatricularAlunAsync(Guid.Parse(aluno),Guid.NewGuid(), StatusMatricula.Ativa);
            //await CriarUsuarioAsync("douglas@gmail.com", "Douglas@2026", "Douglas costa", new DateTime(1998, 12, 31), roleUsuarioId, false);
            //await CriarUsuarioAsync("outro@gmail.com", "Senha@2026", "outro usuario", new DateTime(2000, 06, 07), roleUsuarioId, false);
        }

        private static async Task<string> CriarAlunotesteAsync(AlunoContext alunoContext)
        {
            var alunoId = Guid.NewGuid();
            var aluno = new API.Models.Aluno(
                alunoId,
                DateTime.Now
            );

            alunoContext.Alunos.Add(aluno);

            await alunoContext.SaveChangesAsync();
            return alunoId.ToString();
        }

        private static async Task MatricularAlunAsync(Guid alunoId, Guid cursoId, StatusMatricula status)
        {
            var matricula = Matricula
                .Criar(
                alunoId,
                cursoId,
                codMatricula: 1001,
                status: StatusMatricula.Ativa
               );

            _alunoContext.Matriculas.Add(matricula);

            await _alunoContext.SaveChangesAsync();

        }
    }
}
