using MBA.Conteudo.Domain.ValueObjects;
using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;


namespace MBA.Conteudo.Domain.Entities;

public class Curso : Entity, IAggregateRoot
{
	#region Atributos

	public string Nome { get; private set; } = string.Empty;
	public decimal Valor { get; private set; }
	public bool Ativo { get; private set; }
	public DateTime? ValidoAte { get; private set; } = new();

	public ConteudoProgramatico ConteudoProgramatico { get; private set; } = null!;

	private readonly List<Aula> _aulas = [];
	public IReadOnlyCollection<Aula> Aulas => _aulas.AsReadOnly();

	// EF Constructor
	protected Curso()
	{
		//ConteudoProgramatico = new ConteudoProgramatico(string.Empty, string.Empty);
		_aulas = [];
	}

	public Curso(string nome,
		decimal valor,
		DateTime? validoAte,
		ConteudoProgramatico conteudoProgramatico)
	{
		Nome = nome;
		Valor = valor;
		ValidoAte = validoAte;
		ConteudoProgramatico = conteudoProgramatico;
		Ativo = true;

		ValidarIntegridadeCurso(nome, valor, validoAte, conteudoProgramatico);
	}

	#endregion

	#region Métodos

	public short CargaHoraria()
	{
		return (short)_aulas.Sum(a => a.CargaHoraria);
	}

	public int QuantidadeAulas()
	{
		return _aulas.Count;
	}

	public bool CursoDisponivel()
	{
		return Ativo && (!ValidoAte.HasValue || ValidoAte.Value >= DateTime.Now.Date);
	}

	public Aula ObterAulaPeloId(Guid aulaId)
	{
		var aula = _aulas.FirstOrDefault(a => a.Id == aulaId);
		return aula is null ? throw new DomainException("Aula não encontrada") : aula;
	}

	public void AtivarCurso()
	{
		Ativo = true;
	}

	public void DesativarCurso()
	{
		Ativo = false;
	}

	public void AlterarNome(string nome)
	{
		ValidarIntegridadeCurso(nome);
		Nome = nome;
	}

	public void AlterarValor(decimal valor)
	{
		ValidarIntegridadeCurso(novoValor: valor);
		Valor = valor;
	}

	public void AlterarValidadeCurso(DateTime? validoAte)
	{
		ValidarIntegridadeCurso(novoValidoAte: validoAte);
		ValidoAte = validoAte;
	}

	public void AtualizarConteudoProgramatico(string finalidade, string ementa)
	{
		ConteudoProgramatico = new ConteudoProgramatico(finalidade, ementa);
	}

	public void AdicionarAula(string descricao, short cargaHoraria, byte ordemAula, string url)
	{
		ValidarOrdemAula(Guid.Empty, ordemAula);
		_aulas.Add(new Aula(Id, descricao, cargaHoraria, ordemAula, url));
	}

	public void AdicionarAulaSeedDeDados(string descricao, short cargaHoraria, byte ordemAula, string url,
		Guid aulaId) //USAR SOMENTE PARA SEED DE DADOS
	{
		ValidarOrdemAula(Guid.Empty, ordemAula);
		_aulas.Add(new Aula(Id, aulaId, descricao, cargaHoraria, ordemAula, url));
	}

	public void RemoverAula(Aula aula)
	{
		if (!_aulas.Contains(aula))
			throw new DomainException("Aula não pertence a este curso");

		_aulas.Remove(aula);
	}

	public void AtivarAula(Guid aulaId)
	{
		var aula = ObterAulaPeloId(aulaId);
		aula.AtivarAula();
	}

	public void DesativarAula(Guid aulaId)
	{
		var aula = ObterAulaPeloId(aulaId);
		aula.DesativarAula();
	}

	public void AlterarDescricaoAula(Guid aulaId, string descricao)
	{
		var aula = ObterAulaPeloId(aulaId);
		aula.AlterarDescricao(descricao);
	}

	public void AlterarCargaHorariaAula(Guid aulaId, short cargaHoraria)
	{
		var aula = ObterAulaPeloId(aulaId);
		aula.AlterarCargaHoraria(cargaHoraria);
	}

	public void AlterarOrdemAula(Guid aulaId, byte ordemAula)
	{
		var aula = ObterAulaPeloId(aulaId);
		ValidarOrdemAula(aulaId, ordemAula);
		aula.AlterarOrdemAula(ordemAula);
	}

	public void AlterarUrlAula(Guid aulaId, string url)
	{
		var aula = ObterAulaPeloId(aulaId);
		aula.AlterarUrl(url);
	}

	private void ValidarIntegridadeCurso(string novoNome = "", decimal? novoValor = null,
		DateTime? novoValidoAte = null, ConteudoProgramatico? novoConteudoProgramatico = null)
	{
		var nome = string.IsNullOrEmpty(novoNome) ? Nome : novoNome;
		var valor = novoValor ?? Valor;
		var validoAte = novoValidoAte ?? ValidoAte;
		var conteudoProgramatico = novoConteudoProgramatico ?? ConteudoProgramatico;

		var validacao = new ResultadoValidacao<Curso>();
		ValidacaoTexto.DevePossuirConteudo(nome, "Nome do curso não pode ser vazio ou nulo", validacao);
		ValidacaoTexto.DevePossuirTamanho(nome, 10, 100, "Nome do curso deve ter entre 10 e 100 caracteres", validacao);
		ValidacaoNumerica.DeveSerMaiorQueZero(valor, "Valor do curso deve ser maior que zero", validacao);
		ValidacaoObjeto.DeveEstarInstanciado(conteudoProgramatico, "Conteúdo programático não foi informado",
			validacao);

		if (validoAte.HasValue)
			ValidacaoData.DeveSerValido(validoAte.Value, "Data de validade do curso não foi informada", validacao);

		validacao.DispararExcecaoDominioSeInvalido();
	}

	private void ValidarOrdemAula(Guid aulaId, byte ordemAula)
	{
		var validacao = new ResultadoValidacao<Curso>();

		if (_aulas.Any(a => a.Id != aulaId && a.OrdemAula == ordemAula))
			validacao.AdicionarErro(
				$"Já existe uma aula com a ordem {ordemAula}. Ordem da aula deve ser única dentro do curso");

		validacao.DispararExcecaoDominioSeInvalido();
	}

	public override string ToString()
	{
		var cursoAtivo = Ativo ? "Sim" : "Não";
		return $"Curso {Nome} com valor de {Valor:N2} (Ativo? {cursoAtivo})";
	}

	#endregion
}