using MBA.Aluno.Domain.Entities;
using MBA.Core.Data;
using MBA.Core.Messages;
using Microsoft.EntityFrameworkCore;

namespace MBA.Aluno.Data.Context;

public class AlunoDbContext : DbContext, IUnitOfWork
{
	public AlunoDbContext(DbContextOptions<AlunoDbContext> options) : base(options) { }

	public DbSet<Domain.Entities.Aluno> Alunos { get; set; }
	public DbSet<Matricula> Matriculas { get; set; }
	public DbSet<Certificado> Certificados { get; set; }

	public DbSet<AulaAssistida> AulaAssistidas { get; set; }


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Ignore<Event>();

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AlunoDbContext).Assembly);
		base.OnModelCreating(modelBuilder);
	}

	public async Task<bool> Commit()
	{
		return await base.SaveChangesAsync() > 0;
	}
}