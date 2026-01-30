using MBA.Auth.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace MBA.Auth.Api.Configuration
{
    public static class DataBaseSelectorExtension
    {
        public static void AddDatabaseSelector(this WebApplicationBuilder builder)
        {
            var provider = builder.Configuration["Database:Provider"];

            switch (provider)
            {
                case "Sqlite":
                        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite(builder.Configuration.GetConnectionString("SQLITEConnection")));
                    break;

                case "SqlServer":
                    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

                        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlServer(connectionString));
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Database provider '{provider}' não configurado.");
            }
        }
    }
}
