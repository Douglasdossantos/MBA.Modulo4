using MBA.Conteudo.Api.Models;
using MBA.Conteudo.Api.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace MBA.Conteudo.Api.Data
{
    public static class ConteudoContextSeed
    {
        public static void Seed(this ConteudoContext context)
        {
            // Verifica se já existe dados
            if (context.Cursos.Any())
            {
                Console.WriteLine("==> Banco já possui dados de Conteúdo");
                return;
            }

            Console.WriteLine("==> Criando dados iniciais de Conteúdo...");

            // Curso 1: Fundamentos de C#
            var cursoFundamentosCSharp = new Curso(
                nome: "Fundamentos de C# e .NET",
                valor: 299.90m,
                validoAte: DateTime.Now.AddYears(1),
                conteudoProgramatico: new ConteudoProgramatico(
                    finalidade: "Ensinar os fundamentos da linguagem C# e do ecossistema .NET",
                    ementa: "Introdução ao C#, tipos de dados, estruturas de controle, POO, LINQ, async/await, etc."
                )
            );

            // Garante que o Id do curso seja válido antes de adicionar aulas
            cursoFundamentosCSharp.Id = Guid.NewGuid();

            cursoFundamentosCSharp.AdicionarAula(
                descricao: "Introdução ao C# e .NET",
                cargaHoraria: 2,
                ordemAula: 1,
                url: "https://exemplo.com/videos/csharp-intro"
            );

            cursoFundamentosCSharp.AdicionarAula(
                descricao: "Tipos de Dados e Variáveis",
                cargaHoraria: 3,
                ordemAula: 2,
                url: "https://exemplo.com/videos/tipos-dados"
            );

            cursoFundamentosCSharp.AdicionarAula(
                descricao: "Estruturas de Controle",
                cargaHoraria: 2,
                ordemAula: 3,
                url: "https://exemplo.com/videos/estruturas-controle"
            );

            cursoFundamentosCSharp.AdicionarAula(
                descricao: "Programação Orientada a Objetos",
                cargaHoraria: 4,
                ordemAula: 4,
                url: "https://exemplo.com/videos/poo"
            );

            context.Cursos.Add(cursoFundamentosCSharp);

            // Curso 2: ASP.NET Core
            var cursoAspNetCore = new Curso(
                nome: "ASP.NET Core - Desenvolvimento Web",
                valor: 499.90m,
                validoAte: DateTime.Now.AddYears(1),
                conteudoProgramatico: new ConteudoProgramatico(
                    finalidade: "Desenvolver aplicações web completas com ASP.NET Core",
                    ementa: "MVC, Web API, Entity Framework, Identity, autenticação, autorização, boas práticas de REST e deploy em produção com ASP.NET Core"
                )
            );

            cursoAspNetCore.Id = Guid.NewGuid();

            cursoAspNetCore.AdicionarAula(
                descricao: "Introdução ao ASP.NET Core",
                cargaHoraria: 2,
                ordemAula: 1,
                url: "https://exemplo.com/videos/aspnet-intro"
            );

            cursoAspNetCore.AdicionarAula(
                descricao: "Criando sua primeira Web API",
                cargaHoraria: 3,
                ordemAula: 2,
                url: "https://exemplo.com/videos/primeira-api"
            );

            cursoAspNetCore.AdicionarAula(
                descricao: "Entity Framework Core",
                cargaHoraria: 4,
                ordemAula: 3,
                url: "https://exemplo.com/videos/ef-core"
            );

            cursoAspNetCore.AdicionarAula(
                descricao: "Autenticação e Autorização",
                cargaHoraria: 3,
                ordemAula: 4,
                url: "https://exemplo.com/videos/auth"
            );

            cursoAspNetCore.AdicionarAula(
                descricao: "Deploy na Azure",
                cargaHoraria: 2,
                ordemAula: 5,
                url: "https://exemplo.com/videos/deploy-azure"
            );

            context.Cursos.Add(cursoAspNetCore);

            // Curso 3: Microsserviços com .NET
            var cursoMicrosservicos = new Curso(
                nome: "Arquitetura de Microsserviços com .NET",
                valor: 799.90m,
                validoAte: DateTime.Now.AddYears(1),
                conteudoProgramatico: new ConteudoProgramatico(
                    finalidade: "Construir sistemas distribuídos usando arquitetura de microsserviços",
                    ementa: "Design de microsserviços, comunicação, mensageria, containers, orquestração"
                )
            );

            cursoMicrosservicos.Id = Guid.NewGuid();

            cursoMicrosservicos.AdicionarAula(
                descricao: "Fundamentos de Microsserviços",
                cargaHoraria: 3,
                ordemAula: 1,
                url: "https://exemplo.com/videos/micro-intro"
            );

            cursoMicrosservicos.AdicionarAula(
                descricao: "Comunicação entre Serviços",
                cargaHoraria: 4,
                ordemAula: 2,
                url: "https://exemplo.com/videos/comunicacao"
            );

            cursoMicrosservicos.AdicionarAula(
                descricao: "RabbitMQ e Mensageria",
                cargaHoraria: 4,
                ordemAula: 3,
                url: "https://exemplo.com/videos/rabbitmq"
            );

            cursoMicrosservicos.AdicionarAula(
                descricao: "Docker e Containers",
                cargaHoraria: 3,
                ordemAula: 4,
                url: "https://exemplo.com/videos/docker"
            );

            cursoMicrosservicos.AdicionarAula(
                descricao: "Kubernetes Básico",
                cargaHoraria: 4,
                ordemAula: 5,
                url: "https://exemplo.com/videos/kubernetes"
            );

            context.Cursos.Add(cursoMicrosservicos);

            // Curso 4: Testes Automatizados (Desativado para teste)
            var cursoTestes = new Curso(
                nome: "Testes Automatizados em .NET",
                valor: 349.90m,
                validoAte: DateTime.Now.AddMonths(6),
                conteudoProgramatico: new ConteudoProgramatico(
                    finalidade: "Dominar testes unitários e de integração",
                    ementa: "Introdução a testes automatizados com xUnit, uso de Moq para criação de doubles, testes de integração, TDD e boas práticas de testes em projetos .NET"
                )
            );

            cursoTestes.Id = Guid.NewGuid();

            cursoTestes.AdicionarAula(
                descricao: "Introdução aos Testes",
                cargaHoraria: 2,
                ordemAula: 1,
                url: "https://exemplo.com/videos/testes-intro"
            );

            cursoTestes.AdicionarAula(
                descricao: "Testes Unitários com xUnit",
                cargaHoraria: 3,
                ordemAula: 2,
                url: "https://exemplo.com/videos/xunit"
            );

            cursoTestes.AdicionarAula(
                descricao: "Mocking com Moq",
                cargaHoraria: 2,
                ordemAula: 3,
                url: "https://exemplo.com/videos/moq"
            );

            // Desativar este curso para ter um exemplo de curso desativado
            cursoTestes.DesativarCurso();
            context.Cursos.Add(cursoTestes);

            // Salvar no banco
            context.SaveChanges();

            Console.WriteLine("==> Dados de Conteúdo criados com sucesso!");
            Console.WriteLine($"==> {context.Cursos.Count()} cursos criados");
            Console.WriteLine($"==> {context.Aulas.Count()} aulas criadas");
        }
    }
}
