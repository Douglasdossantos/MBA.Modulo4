using MBA.Core.DomainValidations;

namespace MBA.Conteudo.Api.Models.ValueObjects
{
    public class ConteudoProgramatico
    {

        public string Finalidade { get; } = string.Empty;
        public string Ementa { get; } = string.Empty;

        // EF Constructor
        protected ConteudoProgramatico() { }

        public ConteudoProgramatico(string finalidade, string ementa)
        {
            Finalidade = finalidade;
            Ementa = ementa;

            var validacao = new ResultadoValidacao<ConteudoProgramatico>();
            ValidacaoTexto.DevePossuirConteudo(finalidade, "Finalidade não pode ser vazia ou nula", validacao);
            ValidacaoTexto.DevePossuirTamanho(finalidade, 10, 100, "Finalidade do conteúdo programático deve ter entre 10 e 100 caracteres", validacao);
            ValidacaoTexto.DevePossuirConteudo(ementa, "Ementa do conteúdo programático não pode ser vazia ou nula", validacao);
            ValidacaoTexto.DevePossuirTamanho(ementa, 50, 4000, "Ementa do conteúdo programático deve ter entre 50 e 4000 caracteres", validacao);

            validacao.DispararExcecaoDominioSeInvalido();

        }

        public override string ToString()
        {
            return $"Finalidade: {Finalidade}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Finalidade, Ementa);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ConteudoProgramatico other) return false;
            return Finalidade == other.Finalidade && Ementa == other.Ementa;
        }

        private void ValidarIntegridadeConteudoProgramatico(string novaFinalidade = "", string novaEmenta = "")
        {
            var finalidade = novaFinalidade ?? Finalidade;
            var ementa = novaEmenta ?? Ementa;

            var validacao = new ResultadoValidacao<ConteudoProgramatico>();
            ValidacaoTexto.DevePossuirConteudo(finalidade, "Finalidade não pode ser vazia ou nula", validacao);
            ValidacaoTexto.DevePossuirTamanho(finalidade, 10, 100, "Finalidade do conteúdo programático deve ter entre 10 e 100 caracteres", validacao);
            ValidacaoTexto.DevePossuirConteudo(ementa, "Ementa do conteúdo programático não pode ser vazia ou nula", validacao);
            ValidacaoTexto.DevePossuirTamanho(ementa, 50, 4000, "Ementa do conteúdo programático deve ter entre 50 e 4000 caracteres", validacao);

            validacao.DispararExcecaoDominioSeInvalido();
        }
    }
}
