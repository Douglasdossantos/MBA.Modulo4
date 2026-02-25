using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MBA.Aluno.API.Data.Mapping
{
    public class AlunoMapping : IEntityTypeConfiguration<Models.Aluno>
    {
        public void Configure(EntityTypeBuilder<Models.Aluno> builder)
        {
            builder.ToTable("Alunos");

            builder.HasKey(x => x.Id);

            builder.Property(a => a.CriadoEm)
            .IsRequired()
            .HasColumnType("datetime2")
            .HasDefaultValueSql("SYSDATETIME()");

            builder.HasMany(a => a.Matriculas)
            .WithOne(m => m.Aluno)
            .HasForeignKey(m => m.AlunoId);
        }
    }
}
