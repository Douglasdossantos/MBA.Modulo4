using FluentAssertions;

using MBA.Pagamentos.Application.Queries.ObterPagamento;
using MBA.Pagamentos.Data.Contexts;
using MBA.Pagamentos.Domain.Entities;
using MBA.Pagamentos.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;

namespace MBA.Pagamentos.Testes.Applications.Queries;

public class ObterPagamentoQueryHandlerTests
{
	#region Helpers

	private static FaturamentoDbContext CriarContexto()
	{
		var options = new DbContextOptionsBuilder<FaturamentoDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;

		return new FaturamentoDbContext(options);
	}

	private static Pagamento CriarPagamento(Guid matriculaId, decimal valor = 200m)
		=> new(matriculaId, valor, DateTime.Now.AddDays(5));

	#endregion

	[Fact]
	public async Task ObterPorMatricula_deve_retornar_dto_do_pagamento_pendente()
	{
		var matriculaId = Guid.NewGuid();
		await using var contexto = CriarContexto();
		var pagamento = CriarPagamento(matriculaId, 350m);
		contexto.Pagamentos.Add(pagamento);
		await contexto.SaveChangesAsync();
		var handler = new ObterPagamentoQueryHandler(contexto);

		var dto = await handler.Handle(new ObterPagamentoPorMatriculaQuery(matriculaId), CancellationToken.None);

		dto.Should().NotBeNull();
		dto!.Id.Should().Be(pagamento.Id);
		dto.MatriculaCursoId.Should().Be(matriculaId);
		dto.Valor.Should().Be(350m);
		dto.Status.Should().Be("Pendente");
		dto.DataPagamento.Should().BeNull();
	}

	[Fact]
	public async Task ObterPorId_deve_retornar_dto_do_pagamento_aprovado()
	{
		var matriculaId = Guid.NewGuid();
		await using var contexto = CriarContexto();
		var pagamento = CriarPagamento(matriculaId);
		pagamento.ConfirmarPagamento(
			DateTime.Now,
			"TX-123",
			new DadosCartao("4111111111111111", "Jairo A Souza", "12/26", "123"));
		contexto.Pagamentos.Add(pagamento);
		await contexto.SaveChangesAsync();
		var handler = new ObterPagamentoQueryHandler(contexto);

		var dto = await handler.Handle(new ObterPagamentoPorIdQuery(pagamento.Id), CancellationToken.None);

		dto.Should().NotBeNull();
		dto!.Id.Should().Be(pagamento.Id);
		dto.Status.Should().Be("Aprovado");
		dto.DataPagamento.Should().NotBeNull();
	}

	[Fact]
	public async Task ObterPorMatricula_inexistente_deve_retornar_null()
	{
		await using var contexto = CriarContexto();
		var handler = new ObterPagamentoQueryHandler(contexto);

		var dto = await handler.Handle(new ObterPagamentoPorMatriculaQuery(Guid.NewGuid()), CancellationToken.None);

		dto.Should().BeNull();
	}

	[Fact]
	public async Task ObterPorId_inexistente_deve_retornar_null()
	{
		await using var contexto = CriarContexto();
		var handler = new ObterPagamentoQueryHandler(contexto);

		var dto = await handler.Handle(new ObterPagamentoPorIdQuery(Guid.NewGuid()), CancellationToken.None);

		dto.Should().BeNull();
	}
}
