using MBA.Auth.Api.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace MBA.Auth.Api.Data
{

    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        protected ApplicationDbContext(){}
        public DbSet<Usuarios> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes().SelectMany(
                          e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(1000)");

            base.OnModelCreating(builder);
        }
    }
}
