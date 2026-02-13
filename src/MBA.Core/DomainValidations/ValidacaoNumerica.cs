namespace MBA.Core.DomainValidations
{
    public static class ValidacaoNumerica
    {
        #region Validação BYTE
        public static void DeveSerMaiorQueZero<T>(byte valor, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            if (valor <= 0)
                resultado.AdicionarErro(mensagem);
        }
        #endregion

        #region Validação SHORT
        public static void DeveSerMaiorQueZero<T>(short valor, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            if (valor <= 0)
                resultado.AdicionarErro(mensagem);
        }
        #endregion

        #region Validação INT
        public static void DeveSerMaiorQueZero<T>(int valor, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            if (valor <= 0)
                resultado.AdicionarErro(mensagem);
        }

        public static void DeveEstarEntre<T>(int valor, int tamanhoMinimo, int tamanhoMaximo, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            if (valor < tamanhoMinimo || valor > tamanhoMaximo)
                resultado.AdicionarErro(mensagem);
        }
        #endregion

        #region Validação DECIMAL
        public static void DeveSerMaiorQueZero<T>(decimal valor, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            if (valor <= 0)
                resultado.AdicionarErro(mensagem);
        }
        #endregion
    }
}
