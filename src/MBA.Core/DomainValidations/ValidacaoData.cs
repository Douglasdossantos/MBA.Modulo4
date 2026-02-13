namespace MBA.Core.DomainValidations
{
    public static class ValidacaoData
    {
        public static void DeveSerValido<T>(DateTime data, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            if (data == DateTime.MinValue || data == DateTime.MaxValue)
                resultado.AdicionarErro(mensagem);
        }

        public static void DeveSerMenorQue<T>(DateTime dataValidacao, DateTime dataLimite, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            DeveSerValido(dataValidacao, mensagem, resultado);
            DeveSerValido(dataLimite, mensagem, resultado);

            if (dataValidacao.Date > dataLimite.Date)
                resultado.AdicionarErro(mensagem);
        }

        public static void DeveSerMaiorQue<T>(DateTime dataMaior, DateTime dataMenor, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            DeveSerValido(dataMaior, mensagem, resultado);
            DeveSerValido(dataMenor, mensagem, resultado);

            if (dataMenor.Date > dataMaior.Date)
                resultado.AdicionarErro(mensagem);
        }

        public static void DeveTerRangeValido<T>(DateTime dataInicial, DateTime dataFinal, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            DeveSerValido(dataInicial, mensagem, resultado);
            DeveSerValido(dataFinal, mensagem, resultado);

            if (dataInicial.Date > dataFinal.Date)
                resultado.AdicionarErro(mensagem);
        }
    }
}
