using MBA.Conteudo.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.Conteudo.Data.Mappings;

public class AulaMapping : IEntityTypeConfiguration<Aula>
{
	public void Configure(EntityTypeBuilder<Aula> builder)
	{
		builder.HasKey(a => a.Id);

		builder.Property(a => a.Descricao)
			.HasMaxLength(100)
			.IsRequired();

		builder.Property(a => a.CargaHoraria)
			.IsRequired();

		builder.Property(a => a.OrdemAula)
			.IsRequired();

		builder.Property(a => a.Url)
			.IsRequired()
			.HasMaxLength(1024);

		builder.HasOne(a => a.Curso)
			.WithMany(c => c.Aulas)
			.HasForeignKey(a => a.CursoId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(a => a.CursoId);

		builder.ToTable("Aulas");
	}
}
