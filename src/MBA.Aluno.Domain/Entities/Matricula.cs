using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;
using MBA.Core.SharedDto.Aluno.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Domain.Entities
{
    public class Matricula : Entity, IAggregateRoot
    {
        public Matricula(Guid cursoId, Guid alunoId, DateTime dataMatricula, StatusMatricula status)
        {
            CursoId = cursoId;
            AlunoId = alunoId;
            DataMatricula = dataMatricula;
            Status = status;

            ValidarMatricula();
        }

        public Guid CursoId { get; private set; }
        public Guid AlunoId { get; private set; }
        public DateTime DataMatricula { get; private set; }
        public DateTime? DataCursoConcluido { get; private set; }
        public StatusMatricula Status { get; private set; }
        public Aluno? Aluno { get; set; }
        public Certificado? Certificado { get; private set; }


        public void CriarData() => DataMatricula = DateTime.Now;
        public void CriarDataConcluido() => DataCursoConcluido = DateTime.Now;
        public void statusCancelada() => Status = StatusMatricula.Cancelada;
        public void statusPendentePagamento() => Status = StatusMatricula.PendentePagamento;
        public void statusPagamentoRealizado() => Status = StatusMatricula.PagamentoRealizado;
        public void statuConcluido() => Status = StatusMatricula.Concluido;

        public void AlterarCursoId(Guid cursoId)
        {
            ValidarMatricula(_cursoId: cursoId);
            CursoId = cursoId;
        }
        public void AlterarAlunoId(Guid alunoId)
        {
            ValidarMatricula(_alunoId: alunoId);
            AlunoId = alunoId;
        }

        private void ValidarMatricula(Guid? _cursoId = null, Guid? _alunoId = null, DateTime? _dataMatricula = null)
        {
            var cursoId = _cursoId ?? CursoId;
            var alunoId = _alunoId ?? AlunoId;
            var data = _dataMatricula ?? DataMatricula;

            Validacoes.ValidarSeVazio(cursoId, "O ID do curso não pode ser vazio.");
            Validacoes.ValidarSeVazio(alunoId, "O ID do aluno não pode ser vazio.");
            Validacoes.ValidarData(data, "A data da matrícula é inválida.");
        }

        public override string ToString()
        {
            return $"mtricula com aluno{AlunoId}, curso {CursoId}, realizada na data {DataMatricula}, com status{Status}";
        }



    }
}
