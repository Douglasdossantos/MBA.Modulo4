using MBA.Auth.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace MBA.Auth.Api.Configuration
{
    public static class DataBaseSelectorExtension
    {
        public static void AddDatabaseSelector(this WebApplicationBuilder builder)
        {
            var provider = builder.Environment.EnvironmentName;

            switch (provider)
            {
                case "Development":
                        builder.Services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite(builder.Configuration.GetConnectionString("SQLITEConnection")));
                    break;

                default:
                    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                    builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString));
                    break;
            }
        }
    }
}
