using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MBA.Pagamentos.Data.Migrations
{
    /// <inheritdoc />
    public partial class teste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    PagamentoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MatriculaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NumeroCartao = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    NomeTitularCartao = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ValidadeCartao = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    CVVCartao = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CodigoConfirmacaoPagamento = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PagamentosPK", x => x.PagamentoId);
                });

            migrationBuilder.CreateIndex(
                name: "PagamentoDataVencimentoIDX",
                table: "Pagamentos",
                column: "DataVencimento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pagamentos");
        }
    }
}
