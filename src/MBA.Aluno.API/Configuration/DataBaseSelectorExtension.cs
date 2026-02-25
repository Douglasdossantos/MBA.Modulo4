using MBA.Aluno.API.Data;
using Microsoft.EntityFrameworkCore;

namespace MBA.Aluno.API.Configuration
{
    public static class DataBaseSelectorExtension
    {
        public static void AddDatabaseSelector(this WebApplicationBuilder builder)
        {
            var provider = builder.Environment.EnvironmentName;

            switch (provider)
            {
                case "Development":
                    builder.Services.AddDbContext<AlunoContext>(options =>
                    options.UseSqlite(builder.Configuration.GetConnectionString("SQLITEConnection")));
                    break;

                default:
                    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                    builder.Services.AddDbContext<AlunoContext>(options =>
                    options.UseSqlServer(connectionString));
                    break;
            }
        }
    }
}
