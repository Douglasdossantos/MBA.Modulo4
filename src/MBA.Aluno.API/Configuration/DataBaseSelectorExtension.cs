using MBA.Aluno.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace MBA.Aluno.API.Configuration
{
    public static class DataBaseSelectorExtension
    {
        public static void AddDatabaseSelector(this WebApplicationBuilder builder)
        {
            var provider = builder.Environment.EnvironmentName;

            var dbName = builder.Configuration.GetConnectionString("ConnectionStringAluno");

            var dbPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "MBA.Aluno.Data",
                dbName);

            var sqliteConnection = $"Data Source={dbPath}";


            switch (provider)
            {
                case "Development":
                    builder.Services.AddDbContext<AlunoDbContext>(options =>
                    options.UseSqlite(sqliteConnection));
                    break;

                default:
                    var connectionString = builder.Configuration.GetConnectionString("ConnectionStringAluno");

                    builder.Services.AddDbContext<AlunoDbContext>(options =>
                    options.UseSqlServer(connectionString));
                    break;
            }
        }
    }
}
