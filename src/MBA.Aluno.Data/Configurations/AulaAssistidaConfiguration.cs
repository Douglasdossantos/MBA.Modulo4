using MBA.Aluno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.Aluno.Data.Configurations;

public class AulaAssistidaConfiguration : IEntityTypeConfiguration<AulaAssistida>
{
    public void Configure(EntityTypeBuilder<AulaAssistida> builder)
    {
        builder.ToTable("AulaAssistidas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.MatriculaCursoId)
            .IsRequired();

        builder.Property(a => a.AulaId)
            .IsRequired();

        builder.Property(a => a.DataTermino)
            .IsRequired();
    }
}
