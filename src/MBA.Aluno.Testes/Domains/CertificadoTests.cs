using FluentAssertions;

using MBA.Aluno.Domain.Entities;
using MBA.Core.DomainObjects;

using CertificadoDto = MBA.Core.SharedDto.CertificadoDto;

namespace MBA.Aluno.Testes.Domains;

public class CertificadoTests
{
	private static readonly Guid MatriculaIdValido = Guid.NewGuid();

	[Fact]
	public void Deve_criar_certificado_valido()
	{
		var certificado = new Certificado(MatriculaIdValido);

		certificado.Should().NotBeNull();
		certificado.MatriculaId.Should().Be(MatriculaIdValido);
	}

	[Fact]
	public void Nao_deve_criar_certificado_com_matricula_vazia()
	{
		Action act = () => new Certificado(Guid.Empty);

		act.Should().Throw<DomainException>()
			.WithMessage("*ID da matrícula não pode estar vazio*");
	}

	[Fact]
	public void Deve_setar_matricula()
	{
		var certificado = new Certificado(MatriculaIdValido);
		var nova = Guid.NewGuid();

		certificado.SetarMatricula(nova);

		certificado.MatriculaId.Should().Be(nova);
	}

	[Fact]
	public void Deve_definir_path_e_data()
	{
		var certificado = new Certificado(MatriculaIdValido);

		certificado.Path();
		certificado.CriarData();

		certificado.CertificadoPath.Should().NotBeNullOrWhiteSpace();
		certificado.DataCertificado.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
	}

	[Fact]
	public void Deve_lancar_para_path_muito_curto()
	{
		var certificado = new Certificado(MatriculaIdValido);

		Action act = () => certificado.ValidarCertificado(certificadoPath: "curto");

		act.Should().Throw<DomainException>()
			.WithMessage("*caminho do certificado deve ter entre 10 e 2000 caracteres*");
	}

	[Fact]
	public void Deve_lancar_ao_validar_com_matricula_vazia()
	{
		var certificado = new Certificado(MatriculaIdValido);

		Action act = () => certificado.ValidarCertificado(matriculaId: Guid.Empty);

		act.Should().Throw<DomainException>()
			.WithMessage("*ID da matrícula não pode estar vazio*");
	}

	[Fact]
	public void Deve_converter_certificado_para_dto()
	{
		var certificado = new Certificado(MatriculaIdValido);
		certificado.Path();

		CertificadoDto dto = certificado;

		dto.Id.Should().Be(certificado.MatriculaId);
		dto.PathCertificado.Should().Be(certificado.CertificadoPath);
	}

	[Fact]
	public void Deve_converter_certificado_nulo_para_dto_vazio()
	{
		Certificado? certificado = null;
		CertificadoDto dto = certificado;

		dto.Should().NotBeNull();
	}
}
