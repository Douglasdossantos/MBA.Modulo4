using MBA.Conteudo.Api.Models;
using MBA.Core.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.Conteudo.Api.Data.Mappings
{
    public class CursoMapping : IEntityTypeConfiguration<Curso>
    {
        public void Configure(EntityTypeBuilder<Curso> builder)
        {

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Nome)
                .HasColumnType("varchar(250)")
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Valor)
                .IsRequired()
                .HasPrecision(10, 2);

            builder.OwnsOne(c => c.ConteudoProgramatico, cp =>
            {
                cp.Property(c => c.Finalidade)
                    .HasColumnName("Finalidade")
                    .HasColumnType(DatabaseTypeConstant.Varchar)
                    .HasMaxLength(100)
                    .IsRequired();

                cp.Property(c => c.Ementa)
                    .HasColumnName("Ementa")
                    .HasColumnType(DatabaseTypeConstant.Varchar)
                    .HasMaxLength(4000)
                    .IsRequired();
            });

            builder.ToTable("Cursos");
        }
    }
}
