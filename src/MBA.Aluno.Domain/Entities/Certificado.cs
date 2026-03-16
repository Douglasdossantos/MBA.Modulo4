using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Domain.Entities
{
    public class Certificado : Entity, IAggregateRoot
    {

        public Guid MatriculaId { get; private set; }
        public DateTime DataCertificado { get; private set; }
        public string CertificadoPath { get; private set; }


        public void CriarData() => DataCertificado = DateTime.Now;
        public void Path() => CertificadoPath = "https://marketplace.canva.com/EAFWtfZUSl0/1/0/1600w/canva-certificado-de-participa%C3%A7%C3%A3o-no-curso-azul-claro-e-azul-escuro-73VU6Tj6QUg.jpg";
        public void SetarMatricula(Guid IdMaticula) => MatriculaId = IdMaticula;

        public Certificado() { }

        public Certificado(Guid matriculaId)
        {
            MatriculaId = matriculaId;
            ValidarCertificado();
        }

        public void ValidarCertificado(Guid? _matriculaId = null, string? _certificadoPath = null)
        {
            var matriculaId = _matriculaId ?? MatriculaId;
            var certificadoPath = _certificadoPath ?? CertificadoPath;

            Validacoes.ValidarSeVazio(matriculaId, "O ID da matrícula não pode estar vazio.");
            if (!string.IsNullOrWhiteSpace(certificadoPath))
            {
                Validacoes.ValidarTamanho(certificadoPath, 10, 2000, "O caminho do certificado deve ter entre 10 e 2000 caracteres.");
            }
        }

        public override string ToString()
        {
            return $"Certificado: MatriculaId={MatriculaId}, DataCertificado={DataCertificado:dd/MM/yyyy}, Path={CertificadoPath ?? "Não definido"}";
        }





    }
}
