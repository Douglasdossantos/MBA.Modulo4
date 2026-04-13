namespace MBA.Conteudo.Api.Settings;

public sealed class AppSettings
{
	public JwtSettings JwtSettings { get; set; } = null!;
	public DatabaseSettings DatabaseSettings { get; set; } = null!;
}

public sealed class DatabaseSettings
{
	public string ConnectionStringIdentity { get; set; } = string.Empty;
	public string ConnectionStringConteudo { get; set; } = string.Empty;
	public string ConnectionStringAluno { get; set; } = string.Empty;
	public string ConnectionStringFaturamento { get; set; } = string.Empty;
}

public sealed class JwtSettings
{
	public string Secret { get; set; } = string.Empty;
	public int ExpirationInHours { get; set; }
	public string Issuer { get; set; } = string.Empty;
	public string Audience { get; set; } = string.Empty;
}
