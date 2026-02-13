using MBA.Core.Constants;
using MBA.Pagamentos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace SaberOnline.Faturamento.Data.Configurations;

[ExcludeFromCodeCoverage]
public class FaturamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        #region Mapping columns
        builder.ToTable("Pagamentos");

        builder.HasKey(x => x.Id)
                .HasName("PagamentosPK");

        builder.Property(x => x.Id)
            .HasColumnName("PagamentoId")
            .HasColumnType(DatabaseTypeConstant.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.MatriculaId)
            .HasColumnName("MatriculaId")
            .HasColumnType(DatabaseTypeConstant.UniqueIdentifier)
            .IsRequired();

        builder.Property(x => x.Valor)
            .HasColumnName("Valor")
            .HasColumnType(DatabaseTypeConstant.Money)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.DataVencimento)
            .HasColumnName("DataVencimento")
            .HasColumnType(DatabaseTypeConstant.SmallDateTime)
            .IsRequired();

        builder.Property(x => x.DataPagamento)
            .HasColumnName("DataPagamento")
            .HasColumnType(DatabaseTypeConstant.SmallDateTime);

        builder.Property(c => c.CodigoConfirmacaoPagamento)
        .HasColumnName("CodigoConfirmacaoPagamento")
        .HasColumnType(DatabaseTypeConstant.Varchar)
        .HasMaxLength(100)
        .UseCollation(DatabaseTypeConstant.Collate)
        .IsRequired(false); 

        builder.OwnsOne(c => c.Cartao, cc =>
        {
            cc.Property(c => c.Numero)
                .HasColumnName("NumeroCartao")
                .HasColumnType(DatabaseTypeConstant.Varchar)
                .HasMaxLength(16)
                .UseCollation(DatabaseTypeConstant.Collate);

            cc.Property(c => c.NomeTitular)
                .HasColumnName("NomeTitularCartao")
                .HasColumnType(DatabaseTypeConstant.Varchar)
                .HasMaxLength(50)
                .UseCollation(DatabaseTypeConstant.Collate);

            cc.Property(c => c.Validade)
                .HasColumnName("ValidadeCartao")
                .HasColumnType(DatabaseTypeConstant.Varchar)
                .HasMaxLength(5)
                .UseCollation(DatabaseTypeConstant.Collate);

            cc.Property(c => c.CVV)
                .HasColumnName("CVVCartao")
                .HasColumnType(DatabaseTypeConstant.Varchar)
                .HasMaxLength(3)
                .UseCollation(DatabaseTypeConstant.Collate);
        });


        builder.OwnsOne(c => c.StatusPagamento, sp =>
        {
            sp.Property(c => c.Status)
                .HasColumnName("Status")
                .HasColumnType(DatabaseTypeConstant.Byte)
                .IsRequired();
        });
        #endregion Mapping columns

       
        builder.HasIndex(x => x.DataVencimento).HasDatabaseName("PagamentoDataVencimentoIDX");
        

        
    }
}