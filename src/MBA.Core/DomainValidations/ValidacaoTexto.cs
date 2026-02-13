using System.Text.RegularExpressions;

namespace MBA.Core.DomainValidations
{
    public static class ValidacaoTexto
    {
        public static void DeveSerDiferenteDe<T>(string valor, string comparacao, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            if (valor != comparacao)
                resultado.AdicionarErro(mensagem);
        }

        public static void DevePossuirConteudo<T>(string valor, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            if (string.IsNullOrWhiteSpace(valor))
                resultado.AdicionarErro(mensagem);
        }

        public static void DevePossuirTamanho<T>(string valor, int tamanhoMinimo, int? tamanhoMaximo, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            tamanhoMinimo = tamanhoMinimo == 0 ? 1 : tamanhoMinimo;

            if ((valor?.Length ?? 0) < tamanhoMinimo || (tamanhoMaximo.HasValue && valor.Length > tamanhoMaximo.Value))
                resultado.AdicionarErro(mensagem);
        }

        public static void DeveAtenderRegex<T>(string valor, string expressaoRegular, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            var regex = new Regex(expressaoRegular);
            Match match = regex.Match(valor);
            if (!match.Success)
                resultado.AdicionarErro(mensagem);
        }
    }
}
