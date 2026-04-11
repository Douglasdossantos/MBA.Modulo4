using MBA.Aluno.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.Aluno.Data.Configurations;

public class CertificadoConfiguration : IEntityTypeConfiguration<Certificado>
{
    public void Configure(EntityTypeBuilder<Certificado> builder)
    {
        builder.ToTable("Certificados");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.MatriculaId)
            .IsRequired();

        builder.Property(c => c.DataCertificado)
            .IsRequired();

        builder.Property(c => c.CertificadoPath)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasIndex(c => c.MatriculaId)
            .IsUnique()
            .HasDatabaseName("IX_Certificados_MatriculaId");
    }
}
