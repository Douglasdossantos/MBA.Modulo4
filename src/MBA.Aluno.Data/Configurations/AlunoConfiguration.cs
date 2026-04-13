using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.Aluno.Data.Configurations;

public class AlunoConfiguration : IEntityTypeConfiguration<Domain.Entities.Aluno>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Aluno> builder)
    {
        builder.ToTable("Alunos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Ativo)
            .IsRequired();

        builder.Property(a => a.Adm)
            .IsRequired();

        builder.Property(a => a.DataCriacao)
            .IsRequired();

        builder.HasMany(a => a.Matriculas)
            .WithOne(m => m.Aluno)
            .HasForeignKey(m => m.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.Matriculas)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
