using MBA.Aluno.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.Aluno.API.Data.Mapping
{
    public class ProgressoAulaMapping : IEntityTypeConfiguration<ProgressoAula>
    {
        public void Configure(EntityTypeBuilder<ProgressoAula> builder)
        {
            builder.ToTable("ProgressoAulas");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.MatriculaId)
                .IsRequired();

            builder.Property(p => p.AulaId)
                .IsRequired();

            builder.Property(p => p.Concluida)
                .IsRequired();

            builder.Property(p => p.ConcluidaEm)
                .HasColumnType("datetime2");

            builder.HasOne(p => p.Matricula)
                .WithMany() 
                .HasForeignKey(p => p.MatriculaId)
                .HasConstraintName("FK_ProgressoAulas_Matriculas")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => new { p.MatriculaId, p.AulaId })
                .IsUnique();
        }
    }
}
