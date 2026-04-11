using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;
using MBA.Core.SharedDto.Aluno;

using System.Diagnostics.CodeAnalysis;

namespace MBA.Aluno.Domain.Entities;

public class Aluno : Entity, IAggregateRoot
{
	public string Nome { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public bool Ativo { get; set; }
	public bool Adm { get; set; }
	public DateTime DataCriacao { get; private set; }

	[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
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
		ValidarAluno(nome);
		Nome = nome;
	}

	public void AlterarEmail(string email)
	{
		ValidarAluno(email: email);
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

	public void CriarData()
	{
		DataCriacao = DateTime.Now;
	}

	public void CriarDataDeixaAMesma(DateTime data)
	{
		DataCriacao = data;
	}

	public void ValidarAluno(string? nome = null, string? email = null)
	{
		if (!string.IsNullOrWhiteSpace(nome))
			Nome = nome;

		if (!string.IsNullOrWhiteSpace(email))
			Email = email;

		Validacoes.ValidarSeVazio(Id, "O Id do aluno não pode estar vazio");
		Validacoes.ValidarSeVazio(Nome, "O nome do aluno não pode estar vazio");
		Validacoes.ValidarTamanho(Nome, 3, 150, "O nome deve ter entre 3 e 150 caracteres");
		Validacoes.ValidarSeVazio(Email, "O email não pode estar vazio");
		Validacoes.ValidarTamanho(Email, 5, 200, "O email deve ter entre 5 e 200 caracteres");
		Validacoes.ValidarSeIgual(DataCriacao, default(DateTime), "A data de criação é inválida");
	}

	public override string ToString()
	{
		return $"Aluno: {Nome}, Email: {Email}, Ativo: {Ativo}, Adm: {Adm}, Criado em: {DataCriacao}";
	}

	public static implicit operator AlunoDto(Aluno? aluno)
	{
		if (aluno is null) return new AlunoDto();

		return new AlunoDto
		{
			Id = aluno.Id,
			Nome = aluno.Nome,
			Email = aluno.Email,
			Ativo = aluno.Ativo,
			Adm = aluno.Adm,
			DataCriacao = aluno.DataCriacao,
			Matriculas = aluno.Matriculas.Select(m => (MatriculaDto)m).ToList()
		};
	}
}