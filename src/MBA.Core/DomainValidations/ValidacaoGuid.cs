namespace MBA.Core.DomainValidations
{
    public static class ValidacaoGuid
    {
        public static void DeveSerValido<T>(Guid valor, string mensagem, ResultadoValidacao<T> resultado) where T : class
        {
            if (valor == Guid.Empty)
                resultado.AdicionarErro(mensagem);
        }
    }
}
