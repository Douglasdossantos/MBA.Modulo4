# Plataforma Educacional Distribuída — MBA FullStack Módulo 4

## Grupo de Alunos:

- Jose Ricardo @JOSER1CARDO 
- Leonardo Silva - @Leonardo-Da-Silva-Rocha 
- Douglas dos Santos - @Douglasdossantos 
- Geraldo Alves Simão Junior -  @geraldsimon 
- Silvio Kinaake - @Silviokinaake 
- Diego Lobo - @diegolobo 
- Alberto - @nauthin


## 1. Visão Geral

Este repositório contém a entrega do **Módulo 4 do MBA DevXpert Full Stack .NET**, que propõe a evolução de uma aplicação monolítica para uma **plataforma educacional distribuída** baseada em microsserviços, bounded contexts bem definidos, comunicação síncrona via HTTP e assíncrona via broker.

A solução modela o ciclo de vida completo de um aluno em uma plataforma de cursos online: **cadastro, matrícula, pagamento, acompanhamento de progresso em aulas e conclusão do curso**. Cada responsabilidade vive em um contexto isolado (Auth, Aluno, Conteúdo, Pagamentos) e um **BFF (Backend for Frontend)** orquestra a experiência do usuário final.

A comunicação entre os contextos ocorre de duas formas complementares. As operações **síncronas** (consulta de dados entre contextos e orquestração no BFF) são feitas via HTTP, idealmente com `IHttpClientFactory` + `Refit` + políticas de resiliência Polly (retry exponencial + circuit breaker). As operações **assíncronas de integração** (ex.: confirmação de pagamento acionando a ativação de uma matrícula) são publicadas em **RabbitMQ** através do abstrator `IMessageBus` (EasyNetQ), permitindo que cada serviço evolua de forma independente sem acoplamento direto entre APIs.

## 2. Arquitetura de Serviços

| Serviço | Projeto | Responsabilidade principal | Porta HTTPS (dev) | Porta HTTP (dev) |
|---|---|---|---|---|
| Auth API | `src/MBA.Auth.Api` | Cadastro/login de usuário, emissão de JWT, Identity | `https://localhost:7163` | `http://localhost:5020` |
| Aluno API | `src/MBA.Aluno.API` | Aluno, matrícula, progresso de aulas, conclusão de curso | `https://localhost:7124` | `http://localhost:5236` |
| Conteúdo API | `src/MBA.Conteudo.Api` | Cursos, aulas, categorias | `https://localhost:7285` | `http://localhost:5137` |
| Pagamentos API | `src/MBA.Pagamentos.Api` | Faturamento, transações, integração de gateway | `https://localhost:7171` | `http://localhost:5190` |
| BFF API | `src/MBA.Bff.Api` | Orquestração de chamadas, agregação para o front-end | `https://localhost:7119` | `http://localhost:5289` |

Projetos de suporte:

- `src/MBA.Core` — utilitários transversais: Mediator in-process, eventos de integração, base de Domain Notifications, claims principal.
- `src/MBA.WebApi.Core` — extensões reutilizáveis para Identity, JWT, CORS, Swagger e Polly.
- `src/MBA.MessageBus` — wrapper sobre EasyNetQ (`IMessageBus.PublishAsync`, `SubscribeAsync`).
- `src/MBA.<Contexto>.Application` / `.Data` / `.Domain` — camadas internas por bounded context (Command/Query handlers, DbContext, entidades e regras de domínio).

## 3. Fluxo de Comunicação

```
                          +----------------------+
                          |     Front-end /      |
                          |     WebApp.MVC       |
                          +----------+-----------+
                                     |
                                     v
                          +----------------------+
                          |       BFF API        |
                          |  (Refit/IHttpFactory |
                          |    + Polly)          |
                          +---+---+---+---+------+
                              |   |   |   |
            +-----------------+   |   |   +-----------------+
            v                     v   v                     v
   +-----------------+   +----------------+   +------------------+
   |    Auth API     |   |   Aluno API    |   |   Conteúdo API   |
   +-----------------+   +----------------+   +------------------+
                                 |
                                 | HTTP sync
                                 v
                          +-----------------+
                          | Pagamentos API  |
                          +--------+--------+
                                   |
                                   | publish PagamentoConfirmado/Recusado
                                   v
                          +-----------------+
                          |    RabbitMQ     |   <--- subscribe  Aluno API
                          +-----------------+
```

- **Síncrono (HTTP):** BFF → Auth/Aluno/Conteúdo/Pagamentos. Aluno → Conteúdo (validar curso ativo na matrícula). Pagamentos → Aluno (validar `PagamentoPodeSerRealizado`).
- **Assíncrono (RabbitMQ):** Pagamentos publica `PagamentoConfirmadoEvent` / `PagamentoRecusadoEvent`; Aluno consome e atualiza o status da matrícula.

## 4. Pré-requisitos

- **.NET 9 SDK** (obrigatório — verificar `global.json` / `TargetFramework net9.0`).
- **RabbitMQ 3.x** acessível em `localhost:5672` com credenciais `guest/guest` (padrão Development). Recomendado via Docker: `docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management`.
- **SQLite** (usado em Development — arquivos `.db` gerados automaticamente em cada serviço). **SQL Server** opcional para Production (controlado por `Database__Provider=SqlServer`).
- IDE de sua preferência: **Visual Studio 2022 17.10+**, **JetBrains Rider** ou **VS Code + C# Dev Kit**.
- **Git** para clonar o repositório.

## 5. Configuração

### 5.1. Connection Strings

Cada serviço mantém sua própria connection string em `appsettings.Development.json`:

- Auth API: `ConnectionStrings:DefaultConnection` → `Data Source=Data/AuthDB.db`
- Aluno API: `ConnectionStrings:ConnectionSqliteAluno` → `AlunoDB.db`
- Conteúdo API: `AppSettings:DatabaseSettings:ConnectionStringConteudo` → `Data Source=Data\ConteudoDB.db`
- Pagamentos API: SQLite resolvido em runtime por `SqlitePathResolver` (Development).

Para alternar para SQL Server em Production, use o profile `sql server` ou defina as variáveis:

```bash
export ASPNETCORE_ENVIRONMENT=Production
export Database__Provider=SqlServer
```

### 5.2. RabbitMQ

Todas as APIs que publicam ou consomem eventos usam a mesma connection string em `MessageQueueConnection:MessageBus`:

```
host=localhost:5672;publisherConfirms=true;timeout=30;username=guest;password=guest
```

### 5.3. JWT

Chave simétrica compartilhada entre Auth/Aluno/Conteúdo/Pagamentos em `AppSettings` (ex.: `Secret`, `ExpiracaoHoras`, `Emissor`, `ValidoEm`). Em produção, substituir por variável de ambiente / secrets manager.

### 5.4. URLs do BFF

`AppServicesSettings` no `appsettings.Development.json` do BFF mapeia os serviços (ajustar se as portas forem alteradas):

```
AlunoUrl         = https://localhost:7124
ConteudoUrl      = https://localhost:7285/
PagamentoUrl     = https://localhost:7171
AutenticacaoUrl  = https://localhost:7163/
FaturamentoUrl   = https://localhost:7171/
```

## 6. Ordem de Subida

A ordem recomendada para o ambiente local é:

1. **RabbitMQ** (precisa estar online antes de qualquer API que publique/consuma eventos).
2. **Auth API** — gera tokens usados pelas demais.
3. **Conteúdo API** — catálogo de cursos, pré-requisito para matrícula.
4. **Aluno API** — matrícula/progresso; depende de Conteúdo e escuta eventos do broker.
5. **Pagamentos API** — depende de Aluno (validação) e publica eventos após processar.
6. **BFF API** — porta de entrada para o front-end; depende de todas as anteriores.

Migrations e seeds são executados automaticamente no startup (quando aplicável) via `CarregamentoDadosAsync()` e helpers de `DatabaseSelector`.

## 7. Comandos Rápidos

```bash
# Restaurar e compilar toda a solution
dotnet restore MBA.Modulo4.sln
dotnet build   MBA.Modulo4.sln

# Subir cada serviço (em terminais separados)
dotnet run --project src/MBA.Auth.Api
dotnet run --project src/MBA.Conteudo.Api
dotnet run --project src/MBA.Aluno.API
dotnet run --project src/MBA.Pagamentos.Api
dotnet run --project src/MBA.Bff.Api
```

Swagger disponível em cada serviço na rota `/swagger` (Development).

## 8. Fluxos Principais

### 8.1. Cadastro + Matrícula + Pagamento + Ativação

```
  Usuário        BFF            Auth         Aluno        Conteúdo     Pagamentos     RabbitMQ
    |             |              |             |             |             |            |
    |--register-->|--POST------->|             |             |             |            |
    |             |              |---JWT------>|             |             |            |
    |<--token-----|              |             |             |             |            |
    |             |              |             |             |             |            |
    |--matricula->|--POST------->|             |--valida---->|             |            |
    |             |              |             |<--curso ok--|             |            |
    |             |              |             |--cria matr--|             |            |
    |             |              |             |  (PendentePagamento)      |            |
    |             |              |             |             |             |            |
    |--pay------->|--POST---------------------->|             |--registra->|            |
    |             |              |             |             |             |--publica-->|
    |             |              |             |<---------PagamentoConfirmadoEvent------|
    |             |              |             |--atualiza   |             |            |
    |             |              |             |  status     |             |            |
    |             |              |             |(PagamentoRealizado)       |            |
```

### 8.2. Progresso e Conclusão

1. Aluno assiste uma aula → BFF/Aluno API recebe `RegistrarAulaAssistidaCommand` → grava `ProgressoAula`.
2. `AlunoQueryService` calcula `totalAulas`, `totalAssistidas` e `aulasFaltantes` com base em Conteúdo API + ProgressoAula local.
3. Quando `aulasFaltantes == 0`, `ConcluirCursoCommandHandler` marca a matrícula como concluída e (opcionalmente) emite evento de conclusão.

## 9. Estrutura de Pastas

Cada bounded context segue a convenção:

```
src/
  MBA.<Contexto>.Api            -> projeto ASP.NET Core (Controllers, Configuration, Program)
  MBA.<Contexto>.Application    -> Commands, Queries, Handlers, Validators, DTOs
  MBA.<Contexto>.Domain         -> Entidades, Value Objects, regras de domínio
  MBA.<Contexto>.Data           -> DbContext, Mappings, Migrations, Seed
```

Projetos compartilhados:

```
src/
  MBA.Core            -> Mediator, Events, Messages, AppIdentityUser
  MBA.WebApi.Core     -> JWT, Swagger, Polly, CORS, DatabaseSelector
  MBA.MessageBus      -> IMessageBus (EasyNetQ wrapper)
```

## 10. Troubleshooting

- **RabbitMQ offline** → publicação falha silenciosamente e consumers não sobem. Suba o container antes das APIs e confira o painel em `http://localhost:15672` (`guest/guest`).
- **Porta em uso** → ajuste `applicationUrl` em `Properties/launchSettings.json` do serviço em conflito (e atualize `AppServicesSettings` do BFF).
- **Migrations não aplicadas** → cada API roda a criação automática no startup. Em caso de schema corrompido no SQLite local, remova o arquivo `.db` do serviço e reinicie.
- **401 em rotas protegidas** → confira se o JWT está no header `Authorization: Bearer <token>` e se a `Secret` é igual em todas as APIs.
- **Fluxo de pagamento não ativa matrícula** → verifique (a) se `PagamentoConfirmadoEvent` foi publicado em `IMessageBus.PublishAsync`, (b) se o consumer da Aluno API está registrado como `HostedService`/subscriber e (c) se a fila existe no RabbitMQ.
- **BFF retorna `BaseAddress is null`** → verifique se `AppServicesSettings.AutenticacaoUrl` (e demais URLs) estão preenchidas também no `appsettings.json` base, não apenas em Development.

## 11. Licença e Créditos

Projeto acadêmico do **MBA DevXpert Full Stack .NET — Módulo 4**. Não aceita contribuições externas. Dúvidas ou feedbacks pelo recurso de *Issues*. O arquivo `FEEDBACK.md` é de uso exclusivo do instrutor.
