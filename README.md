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

> **Publicado e automatizado:** a plataforma está no ar em dois ambientes (DEV e Staging) num cluster Kubernetes (k3s), com CI/CD GitOps completo (GitHub Actions + GHCR + Argo CD), segredos 100% fora do repositório via Infisical e ingresso seguro por Cloudflare Tunnel. Detalhes e URLs na **seção 4**.

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

## 4. Ambientes Publicados, CI/CD e GitOps

Além do ambiente local de desenvolvimento, a plataforma roda **publicada e 100% automatizada** em três ambientes num cluster **Kubernetes (k3s)** hospedado na Hetzner, com todos os segredos gerenciados pelo **Infisical** e a esteira de deploy operando no modelo **GitOps pull-based** com **Argo CD**.

### 4.1. Ambientes e URLs públicas

| Aplicação | DEV | Staging | Produção |
|---|---|---|---|
| Web (Portal do Aluno) | [dev-mba-store.dots.dev.br](https://dev-mba-store.dots.dev.br) | [stg-mba-store.dots.dev.br](https://stg-mba-store.dots.dev.br) | [mba-store.dots.dev.br](https://mba-store.dots.dev.br) |
| BFF | [dev-mba-store-bff.dots.dev.br](https://dev-mba-store-bff.dots.dev.br/swagger) | [stg-mba-store-bff.dots.dev.br](https://stg-mba-store-bff.dots.dev.br/swagger) | [mba-store-bff.dots.dev.br](https://mba-store-bff.dots.dev.br/swagger) |
| Identidade (Auth) | [dev-mba-auth-api.dots.dev.br](https://dev-mba-auth-api.dots.dev.br/swagger) | [stg-mba-auth-api.dots.dev.br](https://stg-mba-auth-api.dots.dev.br/swagger) | [mba-auth-api.dots.dev.br](https://mba-auth-api.dots.dev.br/swagger) |
| Alunos | [dev-mba-aluno-api.dots.dev.br](https://dev-mba-aluno-api.dots.dev.br/swagger) | [stg-mba-aluno-api.dots.dev.br](https://stg-mba-aluno-api.dots.dev.br/swagger) | [mba-aluno-api.dots.dev.br](https://mba-aluno-api.dots.dev.br/swagger) |
| Conteúdo | [dev-mba-conteudo-api.dots.dev.br](https://dev-mba-conteudo-api.dots.dev.br/swagger) | [stg-mba-conteudo-api.dots.dev.br](https://stg-mba-conteudo-api.dots.dev.br/swagger) | [mba-conteudo-api.dots.dev.br](https://mba-conteudo-api.dots.dev.br/swagger) |
| Financeiro (Pagamentos) | [dev-mba-financeiro-api.dots.dev.br](https://dev-mba-financeiro-api.dots.dev.br/swagger) | [stg-mba-financeiro-api.dots.dev.br](https://stg-mba-financeiro-api.dots.dev.br/swagger) | [mba-financeiro-api.dots.dev.br](https://mba-financeiro-api.dots.dev.br/swagger) |

Cada ambiente vive num namespace próprio do cluster (`mba-modulo4-dev`, `mba-modulo4` e `mba-modulo4-prd`), com RabbitMQ dedicado e bancos SQL Server isolados por serviço e por ambiente (`mba-{serviço}-{dev|staging|prd}`).

### 4.2. Segredos com Infisical (zero segredo no repositório)

- Chave JWT, credenciais do RabbitMQ e connection strings **não existem mais no código nem no histórico de configuração**: vivem num cofre **Infisical self-hosted** (`infisical.dots.dev.br`), separadas por ambiente.
- No cluster, o **Infisical Secrets Operator** sincroniza o cofre para Secrets do Kubernetes e dispara **rolling restart automático** dos Deployments quando um segredo muda (annotation `secrets.infisical.com/auto-reload`).
- Cada API valida os segredos obrigatórios no startup (**fail-fast**): sem eles, a aplicação nem sobe e explica exatamente o que falta e como configurar.
- No desenvolvimento local, o profile `Infisical (dev)` injeta os segredos no F5 via Infisical CLI (ver seção 5).

### 4.3. Esteira CI/CD (GitOps pull-based)

```
merge na develop ──► GitHub Actions
                      ├─ CI: build + testes (.NET 8)
                      └─ CD: builda as 6 imagens ──► GHCR (ghcr.io, imagens privadas)
                            └─ atualiza k8s/dev/ com a nova tag [skip ci]
                                          │
                                          ▼
                               Argo CD (roda DENTRO do k3s)
                               detecta o commit e sincroniza
                                          │
                                          ▼
                            namespace mba-modulo4-dev (ambiente DEV)

merge na master ──► mesmo fluxo com tags stg-<sha> em k8s/staging/ E k8s/prd/
                    └──► ambientes Staging e Produção (produção promove a MESMA
                         imagem validada no staging — sem rebuild)
```

- **O GitHub nunca acessa o cluster**: o Argo CD observa o repositório e **puxa** as mudanças (GitOps pull-based). Nenhuma credencial de cluster existe fora dele.
- As imagens são publicadas no **GitHub Container Registry** usando apenas o `GITHUB_TOKEN` nativo do Actions (zero secrets manuais na esteira) e puxadas pelo cluster via `imagePullSecret`.
- Sync automático com `prune` e `selfHeal`: o estado do cluster converge sempre para o que está no git. **Rollback = `git revert`**.

### 4.4. Infraestrutura e segurança de rede

- **k3s** (Kubernetes) num servidor Hetzner; a API do cluster é restrita por firewall.
- Ingresso público **exclusivamente via Cloudflare Tunnel** (containers `cloudflared` dentro do cluster fazem conexão de saída): nenhuma porta de aplicação aberta no servidor, TLS e proteção DDoS na borda da Cloudflare.
- Painéis de operação, protegidos por **Cloudflare Access** (login por One-Time PIN no e-mail autorizado):
  - **Argo CD** (estado dos deploys, diff, histórico e rollback): `k3s-argocd.dots.dev.br`
  - **Headlamp** (pods, logs, eventos e recursos do cluster): `k3s-panel.dots.dev.br`
- Schema e seed dos bancos são criados automaticamente em Development/Staging no startup (`EnsureCreated` no SQL Server); **Production não cria schema nem seed por design** — os bancos `mba-*-prd` foram inicializados uma única vez, de forma controlada, e a aplicação em produção apenas os consome.

### 4.5. Swagger aberto de propósito

**Todos os ambientes (inclusive produção) expõem o Swagger** para facilitar a consulta e a correção deste trabalho acadêmico. A equipe sabe que documentação interativa não deve ficar pública em uma aplicação real — por isso cada Swagger carrega um aviso explicando a decisão, e ocultá-lo é uma única variável de ambiente: `SWAGGER_ENABLED=false`.

## 5. Pré-requisitos

- **.NET 8 SDK** (obrigatório — todos os projetos usam `TargetFramework net8.0`).
- **RabbitMQ 3.x** acessível em `localhost:5672` com credenciais `guest/guest` (padrão Development). Recomendado via Docker: `docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management`.
- **SQLite** (usado em Development — arquivos `.db` gerados automaticamente em cada serviço). **SQL Server** opcional para Production (controlado por `Database__Provider=SqlServer`).
- IDE de sua preferência: **Visual Studio 2022 17.10+**, **JetBrains Rider** ou **VS Code + C# Dev Kit**.
- **Git** para clonar o repositório.

> ## ⚠️ ATENÇÃO — LEIA ANTES DE RODAR ⚠️
>
> ### OS SEGREDOS (CHAVE JWT, RABBITMQ, CONNECTION STRINGS) **NÃO FICAM MAIS NO REPOSITÓRIO.**
> ### ELES VÊM DO **INFISICAL**. PARA A APLICAÇÃO SUBIR, VOCÊ PRECISA DE **UMA** DAS OPÇÕES:
>
> **OPÇÃO 1 (recomendada) — INSTALAR O INFISICAL CLI:**
> 1. `winget install infisical`
> 2. `infisical login --domain=https://infisical.dots.dev.br`
> 3. No Visual Studio, selecione o profile **`Infisical (dev)`** e rode (F5).
>    Ou no terminal: `infisical run --env=dev -- dotnet run --project src/MBA.<Serviço>`
>
> **OPÇÃO 2 — CONFIGURAR MANUALMENTE (sem Infisical):** preencha as chaves via
> `dotnet user-secrets set "AppSettings:Secret" "<valor>"` (e demais) ou no
> `appsettings.Development.json`.
>
> **SEM UMA DESSAS, A APLICAÇÃO PARA NO STARTUP** com uma mensagem explicando exatamente o que falta
> (validação fail-fast). Não é bug — é proteção para não rodar com segredo faltando.

## 6. Configuração

### 6.1. Connection Strings

Cada serviço mantém sua própria connection string em `appsettings.Development.json`:

- Auth API: `ConnectionStrings:DefaultConnection` → `Data Source=Data/AuthDB.db`
- Aluno API: `ConnectionStrings:ConnectionSqliteAluno` → `AlunoDB.db`
- Conteúdo API: `AppSettings:DatabaseSettings:ConnectionStringConteudo` → `Data Source=Data\ConteudoDB.db`
- Pagamentos API: SQLite resolvido em runtime por `SqlitePathResolver` (Development).

O provider de banco é decidido pelo ambiente: **Development = SQLite; Staging/Production = SQL Server** (com as connection strings vindas do Infisical). Para forçar SQL Server localmente, defina a variável `DATABASE_PROVIDER=SqlServer` — em builds DEBUG a connection string é substituída automaticamente por `(localdb)\MSSQLLocalDB`, protegendo quem não tem acesso ao servidor publicado.

### 6.2. RabbitMQ

Todas as APIs que publicam ou consomem eventos usam a mesma connection string em `MessageQueueConnection:MessageBus`:

```
host=localhost:5672;publisherConfirms=true;timeout=30;username=guest;password=guest
```

> O `guest/guest` vale apenas para o RabbitMQ local. Nos ambientes publicados a credencial é forte, exclusiva por ambiente e vem do Infisical.

### 6.3. JWT

Chave simétrica compartilhada entre Auth/Aluno/Conteúdo/Pagamentos em `AppSettings` (ex.: `Secret`, `ExpiracaoHoras`, `Emissor`, `ValidoEm`). A chave foi **rotacionada e removida do repositório**: em todos os cenários ela vem do Infisical (localmente via profile `Infisical (dev)`; no cluster via Secrets Operator).

### 6.4. URLs do BFF

`AppServicesSettings` no `appsettings.Development.json` do BFF mapeia os serviços (ajustar se as portas forem alteradas):

```
AlunoUrl         = https://localhost:7124
ConteudoUrl      = https://localhost:7285/
PagamentoUrl     = https://localhost:7171
AutenticacaoUrl  = https://localhost:7163/
FaturamentoUrl   = https://localhost:7171/
```

## 7. Ordem de Subida

A ordem recomendada para o ambiente local é:

1. **RabbitMQ** (precisa estar online antes de qualquer API que publique/consuma eventos).
2. **Auth API** — gera tokens usados pelas demais.
3. **Conteúdo API** — catálogo de cursos, pré-requisito para matrícula.
4. **Aluno API** — matrícula/progresso; depende de Conteúdo e escuta eventos do broker.
5. **Pagamentos API** — depende de Aluno (validação) e publica eventos após processar.
6. **BFF API** — porta de entrada para o front-end; depende de todas as anteriores.

Migrations e seeds são executados automaticamente no startup (quando aplicável) via `CarregamentoDadosAsync()` e helpers de `DatabaseSelector`.

## 8. Comandos Rápidos

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

### 8.1. Ambiente completo via Docker Compose

```bash
docker compose up -d --build
```

Sobe o RabbitMQ e todos os serviços em containers non-root, com healthchecks em
`/health/live` e `/health/ready` em todas as APIs. As URLs internas entre serviços
(Aluno → Conteúdo, Pagamentos → Aluno) já estão configuradas por variável de ambiente.

### 8.2. Smoke Test (fluxo E2E automatizado)

Valida o fluxo completo — registro, login, catálogo, matrícula, pagamento e
confirmação assíncrona via RabbitMQ — contra o ambiente Docker:

```bash
# Script bash: sobe o ambiente via docker compose e executa o fluxo
bash scripts/smoke-test.sh               # sobe o ambiente e deixa no ar ao final
bash scripts/smoke-test.sh --skip-up     # usa um ambiente já em execução
bash scripts/smoke-test.sh --down        # derruba o ambiente (down -v) ao final
bash scripts/smoke-test.sh --timeout 90  # tempo máximo (s) do polling da confirmação

# Testes xUnit de smoke (skipados por padrão para não afetar o CI; exigem ambiente no ar)
EXECUTAR_SMOKE_TESTS=true dotnet test src/MBA.SmokeTests -c Release
```

O script termina com os blocos `RELATORIO` (PASS/FAIL/WARN por passo) e `FINDINGS`
(divergências detectadas em runtime); exit 0 indica fluxo íntegro. As URLs dos serviços
podem ser sobrescritas pelas variáveis `SMOKE_AUTH_URL`, `SMOKE_CONTEUDO_URL`,
`SMOKE_ALUNO_URL`, `SMOKE_PAGAMENTOS_URL` e `SMOKE_BFF_URL`. No GitHub Actions, o
workflow `smoke-test.yml` executa o mesmo fluxo sob demanda (workflow_dispatch).

## 9. Fluxos Principais

### 9.1. Cadastro + Matrícula + Pagamento + Ativação

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

### 9.2. Progresso e Conclusão

1. Aluno assiste uma aula → BFF/Aluno API recebe `RegistrarAulaAssistidaCommand` → grava `ProgressoAula`.
2. `AlunoQueryService` calcula `totalAulas`, `totalAssistidas` e `aulasFaltantes` com base em Conteúdo API + ProgressoAula local.
3. Quando `aulasFaltantes == 0`, `ConcluirCursoCommandHandler` marca a matrícula como concluída e (opcionalmente) emite evento de conclusão.

## 10. Estrutura de Pastas

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

## 11. Troubleshooting

- **RabbitMQ offline** → publicação falha silenciosamente e consumers não sobem. Suba o container antes das APIs e confira o painel em `http://localhost:15672` (`guest/guest`).
- **Porta em uso** → ajuste `applicationUrl` em `Properties/launchSettings.json` do serviço em conflito (e atualize `AppServicesSettings` do BFF).
- **Migrations não aplicadas** → cada API roda a criação automática no startup. Em caso de schema corrompido no SQLite local, remova o arquivo `.db` do serviço e reinicie.
- **401 em rotas protegidas** → confira se o JWT está no header `Authorization: Bearer <token>` e se a `Secret` é igual em todas as APIs.
- **Fluxo de pagamento não ativa matrícula** → verifique (a) se `PagamentoConfirmadoEvent` foi publicado em `IMessageBus.PublishAsync`, (b) se o consumer da Aluno API está registrado como `HostedService`/subscriber e (c) se a fila existe no RabbitMQ.
- **BFF retorna `BaseAddress is null`** → verifique se `AppServicesSettings.AutenticacaoUrl` (e demais URLs) estão preenchidas também no `appsettings.json` base, não apenas em Development.

## 12. Licença e Créditos

Projeto acadêmico do **MBA DevXpert Full Stack .NET — Módulo 4**. Não aceita contribuições externas. Dúvidas ou feedbacks pelo recurso de *Issues*. O arquivo `FEEDBACK.md` é de uso exclusivo do instrutor.
