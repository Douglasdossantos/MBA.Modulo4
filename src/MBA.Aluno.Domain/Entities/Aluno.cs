using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Domain.Entities
{
    public class Aluno : Entity, IAggregateRoot
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public bool Adm { get; set; }
        public DateTime DataCriacao { get; private set; }

        private readonly List<Matricula> _matriculas = [];
        public IReadOnlyCollection<Matricula> Matriculas => _matriculas.AsReadOnly();


        protected Aluno() { }

        public Aluno(Guid id, string nome, string email, bool ativo, bool adm, DateTime dataCriacao)
        {
            Id = id;
            Nome = nome;
            Email = email;
            Ativo = ativo;
            Adm = adm;
            DataCriacao = dataCriacao;

            ValidarAluno();
        }



        public void AlterarNome(string nome)
        {
            ValidarAluno(_nome: nome);
            Nome = nome;
        }

        public void AlterarEmail(string email)
        {
            ValidarAluno(_email: email);
            Email = email;
        }

        public void Ativar()
        {
            Ativo = true;
        }

        public void Desativar()
        {
            Ativo = false;
        }

        public void DefinirAdm(bool adm)
        {
            Adm = adm;
        }

        public void CriarData() => DataCriacao = DateTime.Now;

        public void CriarDataDeixaAMesma(DateTime data) => DataCriacao = data;

        public void ValidarAluno(string? _nome = null, string? _email = null)
        {
            var nome = _nome ?? Nome;
            var email = _email ?? Email;

            Validacoes.ValidarSeVazio(Id, "O Id do aluno não pode estar vazio");
            Validacoes.ValidarSeVazio(nome, "O nome do aluno não pode estar vazio");
            Validacoes.ValidarTamanho(nome, 3, 150, "O nome deve ter entre 3 e 150 caracteres");
            Validacoes.ValidarSeVazio(email, "O email não pode estar vazio");
            Validacoes.ValidarTamanho(email, 5, 200, "O email deve ter entre 5 e 200 caracteres");
            Validacoes.ValidarSeIgual(DataCriacao, default(DateTime), "A data de criação é inválida");
        }

        public override string ToString()
        {
            return $"Aluno: {Nome}, Email: {Email}, Ativo: {Ativo}, Adm: {Adm}, Criado em: {DataCriacao}";
        }







    }
}
