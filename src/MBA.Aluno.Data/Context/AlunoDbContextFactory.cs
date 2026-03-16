using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBA.Aluno.Data.Context
{
    public class AlunoDbContextFactory
        : IDesignTimeDbContextFactory<AlunoDbContext>
    {
        public AlunoDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AlunoDbContext>();

            optionsBuilder.UseSqlite("Data Source=Data/AlunoDB.db");

            return new AlunoDbContext(optionsBuilder.Options);
        }
    }
}
