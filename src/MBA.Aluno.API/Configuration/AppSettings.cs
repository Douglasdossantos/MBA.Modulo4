namespace MBA.Aluno.API.Configuration;

public sealed class AppSettings
{
	public required JwtSettings JwtSettings { get; set; }
	public required DatabaseSettings DatabaseSettings { get; set; }
	public ServicosExternosSettings ServicosExternos { get; set; } = new();
}

public sealed class ServicosExternosSettings
{
	public string ConteudoUrl { get; set; } = string.Empty;
}

public sealed class DatabaseSettings
{
	public required string ConnectionStringAluno { get; set; }
}

public sealed class JwtSettings
{
	public required string Secret { get; set; }
	public required int ExpirationInHours { get; set; }
	public required string Issuer { get; set; }
	public required string Audience { get; set; }
}