using FluentValidation.Results;
using MBA.Aluno.API.Models;
using MBA.Core.Data;
using MBA.Core.DomainObjects;
using MBA.Core.Mediator;
using MBA.Core.Messages;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MBA.Aluno.API.Data
{
    public class AlunoContext: DbContext, IUnitOfWork
    {
        private readonly IMediatorHandler _mediatorHandler;
        public AlunoContext(DbContextOptions<AlunoContext> options, IMediatorHandler mediatorHandler) : base(options)
        {
            _mediatorHandler = mediatorHandler;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }
        public DbSet<Models.Aluno> Alunos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<ProgressoAula> ProgressoAula { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(
                          e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(1000)");

            if (Database.IsSqlServer())
            {
                builder.HasSequence<int>("SequenciaMatricula")
                       .StartsAt(1000)
                       .IncrementsBy(1);

                builder.Entity<Matricula>()
                    .Property(m => m.CodMatricula)
                    .HasDefaultValueSql("NEXT VALUE FOR SequenciaMatricula");
            }

            builder.ApplyConfigurationsFromAssembly(typeof(AlunoContext).Assembly);
            builder.Ignore<Event>();
            builder.Ignore<ValidationResult>();

            base.OnModelCreating(builder);
        }

        public async Task<bool> Commit()
        {
            var sucesso = await base.SaveChangesAsync() > 0;
            if (sucesso)
            {
                await _mediatorHandler.PublicarEventos(this);
            }

            return sucesso;
        }
    }
    public static class MediatorExtension
    {
        public static async Task PublicarEventos<T>(this IMediatorHandler mediator, T ctx) where T: DbContext
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.Notificacoes != null && x.Entity.Notificacoes.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notificacoes)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.LimparEventos());

            var tasks = domainEvents
                .Select(async (domainEvents) =>
                {
                    await mediator.PublicarEvento(domainEvents);
                });

            await Task.WhenAll(tasks);

            //var domainEntities = ctx.ChangeTracker
            //    .Entries<Entity>()
            //    .Where(x => x.Entity.Notificacoes != null &&
            //        x.Entity.Notificacoes.Any())
            //    .ToList();

            //var domainEvents = domainEntities
            //    .SelectMany(x => x.Entity.Notificacoes!)
            //    .Where(e => e != null) 
            //    .ToList();

            //foreach (var domainEvent in domainEvents)
            //{
            //    await mediator.PublicarEvento(domainEvent);
            //}

            //foreach (var entity in domainEntities)
            //{
            //    entity.Entity.LimparEventos();
            //}
        }
    }
}
