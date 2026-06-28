# Plataforma Educacional Distribuída — MBA FullStack Módulo 4

## Grupo de Alunos:

- JoseRicardo @JoseRicardo 
- Leonardo_Silva - @Leonardo_Silva 
- DouglasCosta - @DouglasCosta 
- Geraldo -  @Geraldo 
- Silvio - @Silvio 
- Diego Lobo - @Diego Lobo 
- Alberto - @Alberto


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

- **.NET 8 SDK** (obrigatório — `TargetFramework net8.0` em todos os projetos).
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

## 11. DevOps — Containerização, CI/CD e Kubernetes

> Esta seção cobre a entrega de **DevOps (Módulo 5)** construída sobre a plataforma do Módulo 4:
> empacotamento em containers, orquestração local com Docker Compose, pipelines no GitHub Actions e
> deploy em Kubernetes (Kind), com health checks, logs estruturados e métricas.

### 11.1. Containerização (Docker)

Todos os serviços rodam sobre `mcr.microsoft.com/dotnet/aspnet:8.0` (runtime) e são compilados com
`mcr.microsoft.com/dotnet/sdk:8.0` (build), em **build multi-stage** para imagens finais enxutas. A
imagem de runtime instala `curl` (usado pelos healthchecks do Compose) e cria `/app/data` (volume do
SQLite). Cada container expõe a porta **8080** (`ASPNETCORE_URLS=http://+:8080`).

Há duas formas equivalentes de build, mantidas de propósito:

- **`src/Dockerfile`** — Dockerfile **parametrizado** (`--build-arg PROJECT_NAME=<projeto>`), usado pelo
  Docker Compose e pelos scripts de Kubernetes. Restaura só o projeto-alvo (cache de camadas) e publica
  em Release.
- **`src/<Serviço>/Dockerfile`** — um **Dockerfile por serviço** (sem build-arg), para quem prefere
  buildar um serviço isolado de forma explícita.

```bash
# Build parametrizado (contexto = ./src)
docker build --build-arg PROJECT_NAME=MBA.Auth.Api -t mba-auth-api:latest ./src

# Build por serviço (contexto = ./src)
docker build -f src/MBA.Auth.Api/Dockerfile -t mba-auth-api:latest ./src
```

### 11.2. Orquestração local (Docker Compose)

`docker-compose.yml` sobe **RabbitMQ + as 6 imagens** (Auth, Conteúdo, Aluno, Pagamentos, BFF, WebApp
MVC) na rede `mba-network`, com SQLite persistido em volumes nomeados.

```bash
docker compose up --build      # sobe tudo
docker compose ps              # estado + health
docker compose logs -f bff-api # logs de um serviço
docker compose down            # derruba (use -v para limpar volumes)
```

Pontos de robustez já configurados:

- **Healthcheck por serviço** (`curl -f http://localhost:8080/health/live`) e `restart: unless-stopped`.
- **`depends_on: condition: service_healthy`** — o BFF só sobe quando Auth/Conteúdo/Aluno/Pagamentos estão
  saudáveis; o WebApp MVC depende do BFF + Auth.
- Portas no host configuráveis por env (ex.: `AUTH_API_PORT`, `BFF_API_PORT`); RabbitMQ em `5672`/`15672`.

### 11.3. CI/CD (GitHub Actions)

Workflows em `.github/workflows/`:

- **`ci.yml`** — a cada push/PR: `dotnet restore` → `dotnet build` → `dotnet test` (.NET 8, `ubuntu-latest`).
  Garante que a solution compila e os testes passam antes do merge.
- **`cd.yml`** — build e push das **6 imagens** para o Docker Hub via *matrix*, com tags `:latest` e `:<sha>`.
  Requer os secrets `DOCKERHUB_USERNAME` e `DOCKERHUB_TOKEN` no repositório.
- **`lint.yml`** — `dotnet format --verify-no-changes --severity warn` para manter o padrão de estilo.

### 11.4. Kubernetes (Kind)

Manifests em `k8s/`, pensados para um cluster local **Kind**:

- `namespace.yaml` — namespace `mba`.
- `base/configmap.yaml` (`mba-config`, ex.: `DATABASE_PROVIDER=Sqlite`) e `base/secret.yaml` (`mba-secrets`,
  JWT/credenciais de demonstração).
- `infra/rabbitmq.yaml` e `infra/sqlserver.yaml` — dependências de infraestrutura.
- `services/*.yaml` — **Deployment + Service** de cada um dos 6 serviços, com `livenessProbe` em
  `/health/live`, `readinessProbe` em `/health/ready`, `envFrom` do ConfigMap + Secret, `resources`
  (requests/limits) e volume `emptyDir` para o SQLite. BFF e WebApp MVC expostos via **NodePort**
  (`30093` e `30080`, mapeados pelo `kind-config.yaml` para `8093`/`8080` no host).

```powershell
# Provisiona o cluster Kind 'mba' e aplica os manifests base
./k8s/setup.ps1

# Smoke E2E: build + load das imagens, apply, espera Ready, valida health e testa resiliência
./k8s/smoke-test.ps1
```

Front em `http://localhost:8080` e BFF/Swagger em `http://localhost:8093/swagger` após o smoke.

## 12. Observabilidade

Os três pilares mínimos estão padronizados via `MBA.WebApi.Core` (reuso entre serviços):

- **Health checks** — `MapDefaultHealthChecks()` expõe `/health/live` (liveness) e `/health/ready`
  (readiness, com `AddDbContextCheck` nas APIs que têm banco). Consumidos pelos probes do K8s e pelos
  healthchecks do Compose. Resposta em JSON com status e duração por check.
- **Logs estruturados (JSON)** — todos os `appsettings.json` configuram
  `Logging:Console:FormatterName = Json`, fazendo o ASP.NET Core emitir logs em JSON na saída padrão —
  prontos para ingestão por stacks de log (ELK/Loki) sem parsing frágil.
- **Métricas (Prometheus)** — `UseDefaultMetrics()` (`prometheus-net.AspNetCore`) coleta métricas HTTP
  por request (contagem, duração, status) e expõe **`/metrics`** no formato Prometheus em cada serviço,
  pronto para *scrape* por Prometheus/Grafana.

## 13. Como testar a stack DevOps

Roteiro de verificação ponta a ponta — do build aos três pilares de observabilidade.

### 13.1. Build e testes (equivale ao `ci.yml`)

```powershell
dotnet restore MBA.Modulo4.sln
dotnet build   MBA.Modulo4.sln -c Release
dotnet test    MBA.Modulo4.sln
```

Esperado: build sem erros e a suíte de testes verde.

### 13.2. Subir tudo com Docker Compose

```powershell
docker compose up --build -d
docker compose ps      # aguarde todos ficarem "healthy" (~40s de start_period)
```

Portas no host: Auth `5020`, Conteúdo `5137`, Aluno `5236`, Pagamentos `5190`, BFF `5293`, WebApp MVC `5132`.

> **Dica (PowerShell):** `curl` é alias de `Invoke-WebRequest`. Use **`curl.exe`** nos comandos abaixo para
> chamar o cURL real (saída em texto puro, sem o aviso de segurança do PowerShell).

### 13.3. Observabilidade — os três pilares

**Health checks** — devem responder `200` com JSON (status e duração por check):

```powershell
curl.exe http://localhost:5020/health/live
curl.exe http://localhost:5137/health/ready
```

**Métricas (Prometheus)** — gere tráfego e leia `/metrics`; a saída deve listar `http_request_duration_seconds`,
`http_requests_received_total`, etc., no formato de texto do Prometheus:

```powershell
curl.exe http://localhost:5020/health/live
curl.exe http://localhost:5020/metrics
```

**Logs estruturados (JSON)** — cada linha deve ser um objeto JSON (`{"EventId":...,"LogLevel":...,"Message":...}`):

```powershell
docker compose logs auth-api
```

### 13.4. Build de um serviço isolado (Dockerfile dedicado)

```powershell
docker build -f src/MBA.Auth.Api/Dockerfile -t mba-auth-api:test ./src
docker run --rm -p 8080:8080 mba-auth-api:test
# em outro terminal:
curl.exe http://localhost:8080/health/live
```

### 13.5. Kubernetes (Kind) — smoke E2E e resiliência

```powershell
./k8s/setup.ps1        # cria o cluster Kind 'mba' e aplica os manifests base
./k8s/smoke-test.ps1   # build+load das imagens, apply, espera Ready, valida health e mata um pod (auto-heal)
kubectl -n mba get pods
```

Front em `http://localhost:8080`, BFF/Swagger em `http://localhost:8093/swagger`.

### 13.6. Encerrar

```powershell
docker compose down -v                # derruba o Compose e limpa os volumes
kind delete cluster --name mba        # remove o cluster Kubernetes
```

## 14. Licença e Créditos

Projeto acadêmico do **MBA DevXpert Full Stack .NET — Módulo 4**. Não aceita contribuições externas. Dúvidas ou feedbacks pelo recurso de *Issues*. O arquivo `FEEDBACK.md` é de uso exclusivo do instrutor.
