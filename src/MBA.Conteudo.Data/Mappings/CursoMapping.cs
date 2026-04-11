using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MBA.Conteudo.Domain.Entities;
using MBA.Core.Constants;

namespace MBA.Conteudo.Data.Mappings;

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
				.UseCollation(DatabaseTypeConstant.Collate)
				.IsRequired();

			cp.Property(c => c.Ementa)
				.HasColumnName("Ementa")
				.HasColumnType(DatabaseTypeConstant.Varchar)
				.HasMaxLength(4000)
				.UseCollation(DatabaseTypeConstant.Collate)
				.IsRequired();
		});

		builder.ToTable("Cursos");
	}
}