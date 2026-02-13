namespace MBA.Core.DomainValidations
{
    public static class ValidacaoObjeto
    {
        public static void DeveEstarInstanciado<T>(object valor, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            if (valor == null)
                resultado.AdicionarErro(mensagem);
        }
    }
}
