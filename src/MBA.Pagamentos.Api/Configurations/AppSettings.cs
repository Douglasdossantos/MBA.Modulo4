namespace MBA.Pagamentos.Api.Configurations;

public sealed class AppSettings
{
	public JwtSettings JwtSettings { get; set; } = new();
	public DatabaseSettings DatabaseSettings { get; set; } = new();
}

public sealed class DatabaseSettings
{
	public string ConnectionStringIdentity { get; set; } = string.Empty;
	public string ConnectionStringConteudo { get; set; } = string.Empty;
	public string ConnectionStringAluno { get; set; } = string.Empty;
	public string ConnectionStringFaturamento { get; set; } = string.Empty;
	public string? SqliteFolderPath { get; set; }
}

public sealed class JwtSettings
{
	public string Secret { get; set; } = string.Empty;
	public int ExpirationInHours { get; set; }
	public string Issuer { get; set; } = string.Empty;
	public string Audience { get; set; } = string.Empty;
}
