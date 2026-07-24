using System.Net;
using System.Text;

using FluentAssertions;

using MBA.Bff.Api.Models.Aluno;
using MBA.Bff.Api.Models.Pagamento;
using MBA.Bff.Api.Services.Implementation;
using MBA.Bff.Api.Services.Interface;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace MBA.Bff.Testes;

public class AlunoServiceTests
{
	private readonly Mock<IAlunoExternalService> _alunoExternal = new();
	private readonly Mock<IFaturamentoExternalService> _faturamentoExternal = new();
	private readonly AlunoService _service;

	public AlunoServiceTests()
		=> _service = new AlunoService(_alunoExternal.Object, _faturamentoExternal.Object);

	private static HttpResponseMessage Resposta(HttpStatusCode status, string corpo = "{}")
		=> new(status) { Content = new StringContent(corpo, Encoding.UTF8, "application/json") };

	[Fact]
	public async Task MatriculaPagamento_com_matricula_ok_deve_habilitar_pagamento()
	{
		_alunoExternal
			.Setup(x => x.MatricularAluno(It.IsAny<MatriculaRequest>(), It.IsAny<string>()))
			.ReturnsAsync(Resposta(HttpStatusCode.OK));

		RealizarPagamentoRequest? capturado = null;
		_faturamentoExternal
			.Setup(x => x.RealizarPagamento(It.IsAny<Guid>(), It.IsAny<RealizarPagamentoRequest>(), It.IsAny<string>()))
			.Callback<Guid, RealizarPagamentoRequest, string>((_, req, _) => capturado = req)
			.ReturnsAsync(Resposta(HttpStatusCode.OK, "{\"pago\":true}"));

		var vm = new MatriculaViewModel { AlunoId = Guid.NewGuid(), CursoId = Guid.NewGuid(), Valor = 150m };

		var resultado = await _service.MatriculaPagamento(vm, "meu-token");

		var content = resultado.Should().BeOfType<ContentResult>().Subject;
		content.StatusCode.Should().Be(200);
		content.Content.Should().Contain("pago");
		capturado.Should().NotBeNull();
		capturado!.PagamentoPodeSerRealizado.Should().BeTrue();
	}

	[Fact]
	public async Task MatriculaPagamento_com_matricula_falha_deve_desabilitar_pagamento()
	{
		_alunoExternal
			.Setup(x => x.MatricularAluno(It.IsAny<MatriculaRequest>(), It.IsAny<string>()))
			.ReturnsAsync(Resposta(HttpStatusCode.BadRequest));

		RealizarPagamentoRequest? capturado = null;
		_faturamentoExternal
			.Setup(x => x.RealizarPagamento(It.IsAny<Guid>(), It.IsAny<RealizarPagamentoRequest>(), It.IsAny<string>()))
			.Callback<Guid, RealizarPagamentoRequest, string>((_, req, _) => capturado = req)
			.ReturnsAsync(Resposta(HttpStatusCode.OK));

		var vm = new MatriculaViewModel { AlunoId = Guid.NewGuid(), CursoId = Guid.NewGuid() };

		await _service.MatriculaPagamento(vm, "token");

		capturado.Should().NotBeNull();
		capturado!.PagamentoPodeSerRealizado.Should().BeFalse();
	}

	[Fact]
	public async Task MatriculaPagamento_com_pagamento_nulo_deve_retornar_500()
	{
		_alunoExternal
			.Setup(x => x.MatricularAluno(It.IsAny<MatriculaRequest>(), It.IsAny<string>()))
			.ReturnsAsync(Resposta(HttpStatusCode.OK));
		_faturamentoExternal
			.Setup(x => x.RealizarPagamento(It.IsAny<Guid>(), It.IsAny<RealizarPagamentoRequest>(), It.IsAny<string>()))
			.ReturnsAsync((HttpResponseMessage)null!);

		var vm = new MatriculaViewModel { AlunoId = Guid.NewGuid(), CursoId = Guid.NewGuid() };

		var resultado = await _service.MatriculaPagamento(vm, "token");

		var content = resultado.Should().BeOfType<ContentResult>().Subject;
		content.StatusCode.Should().Be(500);
	}

	[Fact]
	public async Task RealizarAula_deve_registrar_e_retornar_status_do_response()
	{
		var alunoId = Guid.NewGuid();
		_alunoExternal
			.Setup(x => x.ObterPorId(It.IsAny<Guid>(), It.IsAny<string>()))
			.ReturnsAsync(new AlunoViewModel { Id = alunoId });
		_alunoExternal
			.Setup(x => x.RegistrarAulaAssistida(It.IsAny<AulaAssistidaRequest>(), It.IsAny<string>()))
			.ReturnsAsync(Resposta(HttpStatusCode.OK, "{\"aula\":true}"));

		var vm = new AulaAssistidaViewModel
		{
			AlunoId = alunoId,
			MatriculaId = Guid.NewGuid(),
			AulaId = Guid.NewGuid()
		};

		var resultado = await _service.RealizarAula(vm, "token");

		resultado.StatusCode.Should().Be(200);
		resultado.Content.Should().Contain("aula");
	}

	[Fact]
	public async Task AlterarStatusMatricula_deve_retornar_status_do_response()
	{
		_alunoExternal
			.Setup(x => x.AlteraStatusMatricula(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>()))
			.ReturnsAsync(Resposta(HttpStatusCode.OK, "{\"ok\":true}"));

		var resultado = await _service.AlterarStatusMatricula(Guid.NewGuid(), 2, "token");

		resultado.StatusCode.Should().Be(200);
		resultado.Content.Should().Contain("ok");
	}
}
