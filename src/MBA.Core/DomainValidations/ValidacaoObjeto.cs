namespace MBA.Core.DomainValidations;

public static class ValidacaoObjeto
{
	public static void DeveEstarInstanciado<T>(object? valor, string mensagem, ResultadoValidacao<T> resultado)
		where T : class
	{
		if (valor is null)
			resultado.AdicionarErro(mensagem);
	}
}