using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;
using MBA.Core.SharedDto;

namespace MBA.Aluno.Domain.Entities;

public class Certificado : Entity, IAggregateRoot
{
	public Guid MatriculaId { get; private set; }
	public DateTime DataCertificado { get; private set; }
	public string CertificadoPath { get; private set; } = string.Empty;


	public void CriarData()
	{
		DataCertificado = DateTime.Now;
	}

	public void Path()
	{
		CertificadoPath =
			"https://marketplace.canva.com/EAFWtfZUSl0/1/0/1600w/canva-certificado-de-participa%C3%A7%C3%A3o-no-curso-azul-claro-e-azul-escuro-73VU6Tj6QUg.jpg";
	}

	public void SetarMatricula(Guid idMaticula)
	{
		MatriculaId = idMaticula;
	}

	public Certificado() { }

	public Certificado(Guid matriculaId)
	{
		MatriculaId = matriculaId;
		ValidarCertificado();
	}

	public void ValidarCertificado(Guid? matriculaId = null, string? certificadoPath = null)
	{
		var validMatriculaId = matriculaId ?? MatriculaId;
		var validCertificadoPath = certificadoPath ?? CertificadoPath;

		Validacoes.ValidarSeVazio(validMatriculaId, "O ID da matrícula não pode estar vazio.");
		if (!string.IsNullOrWhiteSpace(validCertificadoPath))
			Validacoes.ValidarTamanho(validCertificadoPath, 10, 2000,
				"O caminho do certificado deve ter entre 10 e 2000 caracteres.");
	}

	public override string ToString()
	{
		return
			$"Certificado: MatriculaId={MatriculaId}, DataCertificado={DataCertificado:dd/MM/yyyy}, Path={CertificadoPath}";
	}

	public static implicit operator CertificadoDto(Certificado? certificado)
	{
		if (certificado is null) return new CertificadoDto();

		return new CertificadoDto
		{
			Id = certificado.MatriculaId,
			DataSolicitacao = certificado.DataCertificado,
			PathCertificado = certificado.CertificadoPath
		};
	}
}