using MBA.Pagamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace MBA.Pagamentos.Data.Configurations;

[ExcludeFromCodeCoverage]
public class FaturamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
	public void Configure(EntityTypeBuilder<Pagamento> builder)
	{
		builder.ToTable("Pagamentos");

		builder.HasKey(x => x.Id)
			.HasName("PagamentosPK");

		builder.Property(x => x.Id)
			.HasColumnName("PagamentoId")
			.IsRequired();

		builder.Property(x => x.MatriculaId)
			.HasColumnName("MatriculaId")
			.IsRequired();

		builder.Property(x => x.Valor)
			.HasColumnName("Valor")
			.HasPrecision(10, 2)
			.IsRequired();

		builder.Property(x => x.DataVencimento)
			.HasColumnName("DataVencimento")
			.IsRequired();

		builder.Property(x => x.DataPagamento)
			.HasColumnName("DataPagamento");

		builder.Property(x => x.CodigoConfirmacaoPagamento)
			.HasColumnName("CodigoConfirmacaoPagamento")
			.HasMaxLength(100)
			.IsRequired(false);

		builder.OwnsOne(c => c.Cartao, cc =>
		{
			cc.Property(c => c.Numero)
				.HasColumnName("NumeroCartao")
				.HasMaxLength(16)
				.IsRequired();

			cc.Property(c => c.NomeTitular)
				.HasColumnName("NomeTitularCartao")
				.HasMaxLength(50)
				.IsRequired();

			cc.Property(c => c.Validade)
				.HasColumnName("ValidadeCartao")
				.HasMaxLength(5)
				.IsRequired();

			cc.Property(c => c.Cvv)
				.HasColumnName("CVVCartao")
				.HasMaxLength(3)
				.IsRequired();
		});

		builder.OwnsOne(c => c.StatusPagamento, sp =>
		{
			sp.Property(c => c.Status)
				.HasColumnName("Status")
				.IsRequired();
		});

		builder.HasIndex(x => x.DataVencimento)
			.HasDatabaseName("PagamentoDataVencimentoIDX");
	}
}