using MBA.Aluno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.Aluno.Data.Configurations;

public class MatriculaConfiguration : IEntityTypeConfiguration<Matricula>
{
    public void Configure(EntityTypeBuilder<Matricula> builder)
    {
        builder.ToTable("Matriculas");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.CursoId)
            .IsRequired();

        builder.Property(m => m.AlunoId)
            .IsRequired();

        builder.Property(m => m.DataMatricula)
            .IsRequired();

        builder.Property(m => m.DataCursoConcluido);

        builder.Property(m => m.Status)
            .IsRequired();

        builder.HasIndex(m => m.AlunoId)
            .HasDatabaseName("IX_Matriculas_AlunoId");

        builder.HasOne(m => m.Certificado)
            .WithOne()
            .HasForeignKey<Certificado>(c => c.MatriculaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
