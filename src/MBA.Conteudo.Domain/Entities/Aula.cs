
using MBA.Core.DomainObjects;
using MBA.Core.DomainValidations;
using System.Text.Json.Serialization;

namespace MBA.Conteudo.Domain.Entities
{
    public class Aula : Entity
    {
        #region Atributos
        public Guid CursoId { get; private set; }
        public string Descricao { get; private set; } = string.Empty;
        public bool Ativo { get; private set; }
        public short CargaHoraria { get; private set; }
        public byte OrdemAula { get; private set; }
        public string Url { get; private set; } = string.Empty;

        [JsonIgnore]
        public Curso? Curso { get; set; }

        protected Aula() { }
        public Aula(Guid cursoId,
            string descricao,
            short cargaHoraria,
            byte ordemAula,
            string url)
        {
            CursoId = cursoId;
            Descricao = descricao;
            CargaHoraria = cargaHoraria;
            OrdemAula = ordemAula;
            Url = url;
            Ativo = true;

            ValidarIntegridadeAula();
        }
        #endregion

        #region Metodos do Dominio
        internal void AtivarAula() => Ativo = true;
        internal void DesativarAula() => Ativo = false;

        internal void AlterarDescricao(string descricao)
        {
            ValidarIntegridadeAula(novaDescricao: descricao);
            Descricao = descricao;
        }

        internal void AlterarCargaHoraria(short cargaHoraria)
        {
            ValidarIntegridadeAula(novaCargaHoraria: cargaHoraria);
            CargaHoraria = cargaHoraria;
        }

        internal void AlterarOrdemAula(byte ordemAula)
        {
            ValidarIntegridadeAula(novaOrdemAula: ordemAula);
            OrdemAula = ordemAula;
        }

        internal void AlterarUrl(string url)
        {
            ValidarIntegridadeAula(novoUrl: url);
            Url = url;
        }

        private void ValidarIntegridadeAula(string novaDescricao = "", short? novaCargaHoraria = null, byte? novaOrdemAula = null, string novoUrl = "")
        {
            var descricao = novaDescricao ?? Descricao;
            var cargaHoraria = novaCargaHoraria ?? CargaHoraria;
            var ordemAula = novaOrdemAula ?? OrdemAula;
            var url = novoUrl ?? Url;

            var validacao = new ResultadoValidacao<Aula>();
            ValidacaoGuid.DeveSerValido(CursoId, "Id do curso não pode ser vazio", validacao);
            ValidacaoTexto.DevePossuirConteudo(descricao, "Descrição da aula não pode ser vazia ou nula", validacao);
            ValidacaoTexto.DevePossuirTamanho(descricao, 5, 100, "Descrição da aula deve ter entre 5 e 100 caracteres", validacao);
            ValidacaoNumerica.DeveSerMaiorQueZero(cargaHoraria, "Carga horária deve ser maior que zero", validacao);
            ValidacaoNumerica.DeveEstarEntre(cargaHoraria, 1, 5, "Carga horária deve estar entre 1 e 5 horas", validacao);
            ValidacaoNumerica.DeveSerMaiorQueZero(ordemAula, "Ordem da aula deve ser maior que zero", validacao);
            ValidacaoTexto.DevePossuirConteudo(url, "URL da aula não pode ser vazia ou nula", validacao);
            ValidacaoTexto.DevePossuirTamanho(url, 10, 1024, "Url da aula deve ter entre 10 e 1024 caracteres", validacao);

            validacao.DispararExcecaoDominioSeInvalido();
        }

        public override string ToString()
        {
            string aulaAtiva = Ativo ? "Sim" : "Não";
            return $"Aula {Descricao} com carga horária de {CargaHoraria} e ordem {OrdemAula} (Ativo? {aulaAtiva})";
        }
        #endregion

    }
}
