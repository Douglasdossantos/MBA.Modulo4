using MBA.Aluno.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.Aluno.API.Data.Mapping
{
    public class MatriculaMapping : IEntityTypeConfiguration<Matricula>
    {
        public void Configure(EntityTypeBuilder<Matricula> builder)
        {
            builder.ToTable("Matriculas");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedNever();

            builder.Property(m => m.AlunoId)
                .IsRequired();

            builder.Property(m => m.CursoId)
                .IsRequired();

            builder.Property(m => m.CodMatricula)
                .ValueGeneratedOnAdd()
                .IsRequired();

            builder.Property(m => m.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(m => m.CriadaEm)
                .IsRequired()
                .HasColumnType("datetime2")
                .HasDefaultValueSql("SYSDATETIME()");

            builder.Property(m => m.AtivadaEm)
                .HasColumnType("datetime2");

            builder.HasOne(m => m.Aluno)
                .WithMany(a => a.Matriculas)
                .HasForeignKey(m => m.AlunoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => m.CodMatricula)
                .IsUnique();

            builder.HasIndex(m => new { m.AlunoId, m.CursoId })
                .IsUnique();
        }
    }
}
