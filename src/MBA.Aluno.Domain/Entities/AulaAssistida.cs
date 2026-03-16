using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Domain.Entities
{
    public class AulaAssistida : Entity, IAggregateRoot
    {

        public Guid MatriculaCursoId { get; private set; }
        public Guid AulaId { get; set; }
        public DateTime DataTermino { get; set; }

        public AulaAssistida() { }

        public AulaAssistida(Guid matriculaCursoId, Guid aulaId, DateTime dataTermino)
        {
            MatriculaCursoId = matriculaCursoId;
            AulaId = aulaId;
            DataTermino = dataTermino;

            ValidarAulaAssistida();
        }

        public void AlterarAulaId(Guid aulaId)
        {
            ValidarAulaAssistida(_aulaId: aulaId);
            AulaId = aulaId;
        }

        public void AlterarDataTermino(DateTime dataTermino)
        {
            ValidarAulaAssistida(_dataTermino: dataTermino);
            DataTermino = dataTermino;
        }

        public void ValidarAulaAssistida(Guid? _matriculaCursoId = null, Guid? _aulaId = null, DateTime? _dataTermino = null)
        {
            var matriculaCursoId = _matriculaCursoId ?? MatriculaCursoId;
            var aulaId = _aulaId ?? AulaId;
            var dataTermino = _dataTermino ?? DataTermino;

            Validacoes.ValidarSeVazio(matriculaCursoId, "O ID da matrícula do curso não pode estar vazio.");
            Validacoes.ValidarSeVazio(aulaId, "O ID da aula não pode estar vazio.");
            Validacoes.ValidarData(dataTermino, "A data da matrícula é inválida.");
        }



        public override string ToString()
        {
            return $"mtricula {MatriculaCursoId}, aula {AulaId}, realizada na data {DataTermino}";
        }

    }
}
