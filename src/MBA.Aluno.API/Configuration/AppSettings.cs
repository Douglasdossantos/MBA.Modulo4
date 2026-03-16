namespace MBA.Aluno.API.Configuration
{
    public sealed class AppSettings
    {
        public JwtSettings JwtSettings { get; set; }
        public DatabaseSettings DatabaseSettings { get; set; }
    }

    public sealed class DatabaseSettings
    {
        public string ConnectionStringIdentity { get; set; }
        public string ConnectionStringConteudo { get; set; }
        public string ConnectionStringAluno { get; set; }
        public string ConnectionStringFaturamento { get; set; }
    }

    public sealed class JwtSettings
    {
        public string Secret { get; set; }
        public int ExpirationInHours { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
