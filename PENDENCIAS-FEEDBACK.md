# Pendências do Feedback do Professor

> Análise do estado atual da branch `master` em relação ao feedback técnico recebido.
> Cada item traz o **trecho exato do professor**, o **estado atual no código** e a **justificativa** do porquê ainda não atende ao pedido.

---

## 🔴 Prioridade Alta — Fluxos críticos do domínio distribuído

### 1. Evento de pagamento confirmado não sai para o broker

**Professor disse:**
> "O evento de pagamento confirmado não sai do contexto de Pagamentos como integração assíncrona entre microsserviços; hoje ele é publicado apenas via MediatR interno (`src/MBA.Core/Mediator/MediatorHandler.cs:31-34` e `src/MBA.Pagamentos.Application/Commands/RealizarPagamento/RealizarPagamentoCommandHandler.cs:93-99`), sem consumidor identificado na Aluno API."

**Estado atual:**
- `RealizarPagamentoCommandHandler.cs:107-113` continua chamando apenas `_mediatorHandler.PublicarEventoRaiz(new PagamentoConfirmadoEvent(...))`.
- Nenhuma publicação para RabbitMQ/EasyNetQ (`IMessageBus.PublishAsync`) foi adicionada.

**Por que não atende:**
O MediatR é in-process. O evento nunca cruza o limite do serviço de Pagamentos, então nenhuma outra API pode reagir a ele. A expectativa do módulo 4 é comunicação assíncrona entre bounded contexts via broker.

---

### 2. Aluno API não consome `PagamentoConfirmadoEvent`

**Professor disse:**
> "Não foi encontrada implementação de consumo de `PagamentoConfirmadoEvent` na Aluno API nem rotina que altere a matrícula de `PendentePagamento` para `PagamentoRealizado` após pagamento."

**Estado atual:**
- Não existe handler/consumer em `src/MBA.Aluno.API/Services/` ou equivalente registrando subscrição do evento.
- Matrícula permanece eternamente em `PendentePagamento`.

**Por que não atende:**
Sem esse consumer o ciclo de negócio **matrícula → pagamento → ativação** fica quebrado. É consequência direta do item 1, mas exige trabalho próprio na Aluno API (subscriber + handler + atualização da matrícula).

---

### 3. Fluxo de pagamento rejeitado ausente

**Professor disse:**
> "O fluxo de pagamento rejeitado está ausente. Não foi encontrada classe, publicação ou consumo de evento equivalente a `PagamentoRejeitado`."

**Estado atual:**
- A classe `PagamentoRecusadoEvent` foi criada em `src/MBA.Core/Messages/FaturamentoEvents/PagamentoRecusadoEvent.cs`. ✅ parcial
- **Nenhuma publicação** dispara o evento no handler de pagamento.
- **Nenhum consumer** reage a ele.

**Por que não atende:**
Existe apenas a casca (classe DTO). Sem publicação e sem consumidor o fluxo de rejeição não é observável nem produz efeito colateral (ex.: cancelar/sinalizar a matrícula).

---

### 4. `RegistrarAulaAssistidaCommandHandler` não foi implementado

**Professor disse:**
> "O fluxo de realização de aula está incompleto. Embora haja endpoint para registrar aula assistida na Aluno API, o handler correspondente não foi localizado e o registro do handler está comentado (`src/MBA.Aluno.API/Program.cs:69`)."

**Estado atual:**
- O comando `RegistrarAulaAssistidaCommand` existe em `src/MBA.Core/Messages/AlunoCommands/`.
- A pasta `src/MBA.Aluno.Appplication/Commands/` contém apenas: `AlterarStatusMatricula`, `CadastroAluno`, `ConcluirCurso`, `Matricular`.
- **Não existe** `RegistrarAulaAssistidaCommandHandler`.
- O endpoint em `AlunoController.cs:63-93` despacha o comando via MediatR, mas não há quem o atenda → falha em runtime.

**Por que não atende:**
Endpoint público que lança exceção ao ser chamado é pior do que não existir. Precisa implementar o handler, registrar no DI e persistir `ProgressoAula`.

---

### 5. `AlunoQueryService` retorna progresso fixo = 0

**Professor disse:**
> "O fluxo de finalização de curso possui regra funcional frágil: `AlunoQueryService` comenta todo o cálculo de total de aulas/aulas assistidas/aulas faltantes e retorna porcentagem fixa baseada em 0 (`src/MBA.Aluno.Appplication/Queries/AlunoQueryService.cs:37-55`)."

**Estado atual:**
- Linhas 42-47 continuam com o cálculo real **comentado** (`//int totalAulas`, `//int totalAssistidas`…).
- A porcentagem é sempre calculada com numerador 0.

**Por que não atende:**
Combinado com o item 6, qualquer aluno sem aulas assistidas é elegível para conclusão — bug de regra de negócio. É preciso consultar as aulas reais (integração com Conteúdo API ou snapshot local) e contar os `ProgressoAula` já registrados.

---

### 6. Matrícula não valida curso ativo/disponível

**Professor disse:**
> "A matrícula não valida curso ativo/disponível por integração com a Conteúdo API. A lógica correspondente está comentada em `src/MBA.Aluno.Appplication/Commands/Matricular/MatricularAlunoCommandHandler.cs:21, 26, 31, 43, 103-125`."

**Estado atual:**
Código de consulta ao `ICursoRepository`/Conteúdo API permanece comentado nas mesmas linhas.

**Por que não atende:**
Um aluno pode se matricular em curso inexistente/inativo, gerando matrícula órfã e pagamento de algo que não existe. Precisa chamada HTTP ao Conteúdo API (com retry/timeout) ou cache/leitura do snapshot.

---

### 7. `PagamentoPodeSerRealizado` não é validado

**Professor disse:**
> "O pagamento recebe vários dados da matrícula no payload... mas o handler não consulta a Aluno API nem valida a propriedade `PagamentoPodeSerRealizado`; o comando apenas a carrega e o validator não a usa."

**Estado atual:**
- Propriedade existe em `RealizarPagamentoCommand.cs:12`.
- `RealizarPagamentoCommandValidator.cs` valida só campos de cartão/valor/matrícula id, **não consulta a Aluno API** nem aplica a regra.

**Por que não atende:**
O cliente pode enviar `PagamentoPodeSerRealizado = true` mesmo com matrícula em estado inválido. A validação real precisa consultar a Aluno API (ou receber via contrato assíncrono) para confirmar que a matrícula está `PendentePagamento`.

---

## 🟡 Prioridade Média — Segurança / integração

### 8. `EhAdministrador()` desalinhado com as claims emitidas

**Professor disse:**
> "Há inconsistência entre as claims emitidas e a leitura do perfil administrativo. A Auth API emite claims como `new(\"Administrador\", \"ADM\")` e roles em `role` (`src/MBA.Auth.Api/Controllers/AuthController.cs:152-155, 192-215`), enquanto `AppIdentityUser.EhAdministrador()` procura a claim `nivel` com valor `Admin` (`src/MBA.Core/Authentications/AppIdentityUser.cs:41-50`). Com isso, a identificação de administrador tende a falhar."

**Estado atual:**
`AppIdentityUser.cs:45` continua buscando `claim.Type == "nivel" && claim.Value == "Admin"`, enquanto o token emitido traz `Administrador=ADM` e roles.

**Por que não atende:**
Qualquer rota protegida por `EhAdministrador()` nega admins legítimos. É preciso alinhar a leitura com o que a Auth API emite (ou padronizar a emissão).

---

### 9. `AdminController` do BFF faz login manual dentro da action

**Professor disse:**
> "O endpoint de cadastro de curso no BFF está marcado como `[AllowAnonymous]` (`src/MBA.Bff.Api/Controllers/AdminController.cs:29-31`) e realiza login manual dentro da ação para então chamar a Conteúdo API. Isso contraria o modelo esperado de autenticar uma vez e reutilizar o token nas demais requisições."

**Estado atual:**
- `[AllowAnonymous]` foi removido. ✅ parcial
- A action **ainda chama `_autenticacao.Login()` internamente** antes de `_conteudoService.CadastrarCurso()` (linha 34).

**Por que não atende:**
O BFF deve exigir o token do usuário e apenas repassá-lo via `HttpClientAuthorizationDelegatingHandler`. Login embutido na action é credencial em código/config e anula a autorização por usuário real.

---

### 10. BFF não usa `IHttpClientFactory` nem Polly

**Professor disse:**
> "Os clients do BFF são criados manualmente com `new HttpClient(handler)` em `src/MBA.Bff.Api/Configuration/DependencyInjectionConfig.cs:25-49`, sem `IHttpClientFactory`, sem `AddHttpClient` e sem políticas resilientes."
>
> "Não encontrei uso efetivo de políticas de retry/circuit breaker nas chamadas HTTP entre APIs. A extensão `PollyExtensions` existe, mas não há evidência de aplicação em `HttpClient`/Refit no BFF."

**Estado atual:**
- `DependencyInjectionConfig.cs` continua com `new HttpClient(handler)` nas linhas 30, 38, 47, 56.
- `PollyExtensions` existe em `src/MBA.WebApi.Core/Extensions/` mas **não é referenciado** no BFF.

**Por que não atende:**
Sem factory: risco de socket exhaustion. Sem Polly: qualquer indisponibilidade transitória das APIs quebra o BFF. Precisa trocar para `services.AddHttpClient<T>(...).AddPolicyHandler(...)`.

---

### 11. Pagamentos API sem endpoint de consulta de status

**Professor disse:**
> "A Pagamentos API não implementa consulta de status de pagamento; há apenas POST para registrar pagamento em `src/MBA.Pagamentos.Api/Controllers/FaturamentoController.cs:23-67`."

**Estado atual:**
`FaturamentoController` tem apenas `POST registrar-pagamento`. Sem `GET /faturamento/{id}` ou similar.

**Por que não atende:**
Sem consulta, não há como o BFF/Aluno API/front-end auditarem o estado atual de um pagamento. Precisa de endpoint GET que retorne status (via `FaturamentoDbContext`).

---

### 12. Warnings de build — dependências inconsistentes/vulneráveis

**Professor disse:**
> "Incompatibilidade de versões entre `MediatR.Extensions.Microsoft.DependencyInjection 11.1.0` e `MediatR 14.0.0` em `src/MBA.Pagamentos.Api/MBA.Pagamentos.Api.csproj`, além de vulnerabilidade conhecida no pacote `AutoMapper 16.0.0` em `src/MBA.Aluno.Appplication/MBA.Aluno.Appplication.csproj` e `src/MBA.Pagamentos.Api/MBA.Pagamentos.Api.csproj`."

**Estado atual:**
- `MBA.Pagamentos.Api.csproj` mantém `MediatR 14.0.0` + `MediatR.Extensions.Microsoft.DependencyInjection 11.1.0` (MediatR 12+ não precisa mais desse pacote).
- `AutoMapper 16.0.0` continua nos dois projetos.

**Por que não atende:**
Warning ignorado vira bug depois. O pacote `MediatR.Extensions.Microsoft.DependencyInjection` foi absorvido pelo MediatR 12+ (usar `services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly...)`). AutoMapper precisa ser atualizado para versão sem CVE.

---

## 🟢 Prioridade Baixa — Limpeza e documentação

### 13. Projeto residual `MBA.Aluno.Application` (Class1.cs)

**Professor disse:**
> "Existe um projeto solto e aparentemente residual em `src/MBA.Aluno.Application/MBA.Aluno.Application.csproj`, com TargetFramework net9.0 e apenas `Class1.cs`, sem participação na solution e sem utilidade funcional observável."
>
> "Há inconsistência nominal importante entre `MBA.Aluno.Application` e `MBA.Aluno.Appplication` (triplo p)."

**Estado atual:**
A pasta `src/MBA.Aluno.Application/` com `Class1.cs` ainda existe no filesystem.

**Por que não atende:**
Ruído arquitetural. Qualquer desenvolvedor novo confunde os dois diretórios. Precisa deletar a pasta residual (e idealmente renomear `Appplication` → `Application`, ajustando `.sln` e todos os `ProjectReference`).

---

### 14. `AppSettings.cs` da Aluno API com múltiplas connection strings

**Professor disse:**
> "A Aluno API mantém uma classe de configuração com connection strings para múltiplos contextos (`src/MBA.Aluno.API/Configuration/AppSettings.cs:9-15`), o que não prova acoplamento de banco em runtime, mas indica uma modelagem de configuração pouco delimitada para o bounded context."

**Estado atual:**
`DatabaseSettings` da Aluno API ainda expõe `ConnectionStringIdentity`, `ConnectionStringConteudo`, `ConnectionStringAluno`, `ConnectionStringFaturamento`.

**Por que não atende:**
Cada serviço deve conhecer apenas a própria conexão. Precisa remover as três conexões estranhas ao contexto de Aluno.

---

### 15. `appsettings.json` do BFF sem `AutenticacaoUrl` base

**Professor disse:**
> "`src/MBA.Bff.Api/appsettings.json:9-19` não define `AutenticacaoUrl`; ele aparece apenas em `appsettings.Development.json`. Fora desse ambiente, o client pode ser inicializado com URL vazia."

**Estado atual:**
`appsettings.json` base continua sem a chave. Só `appsettings.Development.json` define.

**Por que não atende:**
Em produção/Staging a URL vem vazia e os HttpClients falham silenciosamente. Precisa adicionar a chave no `appsettings.json` base (mesmo que apontando para placeholder/variável de ambiente).

---

### 16. README.md desatualizado

**Professor disse:**
> "O `README.md` está desalinhado com o projeto real. Ele descreve um 'blog simples com MVC e API RESTful' e um módulo introdutório (`README.md:2-103`), não a plataforma educacional distribuída do módulo atual."

**Estado atual:**
README.md ainda fala em "Blog Simples com MVC" e "Introdução ao Desenvolvimento ASP.NET Core".

**Por que não atende:**
Avaliação depende do README para entender como subir o ambiente. Precisa reescrever descrevendo: arquitetura (5 APIs + BFF), pré-requisitos (RabbitMQ, .NET), portas, ordem de subida, fluxos principais, seed automático.

---

## Resumo rápido

| # | Item | Prioridade |
|---|---|---|
| 1 | Publicar `PagamentoConfirmadoEvent` no broker | 🔴 Alta |
| 2 | Consumer de `PagamentoConfirmadoEvent` na Aluno API | 🔴 Alta |
| 3 | Publicar + consumir `PagamentoRecusadoEvent` | 🔴 Alta |
| 4 | Implementar `RegistrarAulaAssistidaCommandHandler` | 🔴 Alta |
| 5 | Calcular progresso real em `AlunoQueryService` | 🔴 Alta |
| 6 | Validar curso ativo ao matricular | 🔴 Alta |
| 7 | Validar `PagamentoPodeSerRealizado` | 🔴 Alta |
| 8 | Alinhar `EhAdministrador()` com claims reais | 🟡 Média |
| 9 | Remover login manual do `AdminController` do BFF | 🟡 Média |
| 10 | `IHttpClientFactory` + Polly no BFF | 🟡 Média |
| 11 | Endpoint GET de status em Pagamentos | 🟡 Média |
| 12 | Corrigir MediatR e AutoMapper | 🟡 Média |
| 13 | Deletar `MBA.Aluno.Application` residual | 🟢 Baixa |
| 14 | Limpar `AppSettings.cs` da Aluno API | 🟢 Baixa |
| 15 | `AutenticacaoUrl` em `appsettings.json` base do BFF | 🟢 Baixa |
| 16 | Reescrever README.md | 🟢 Baixa |

## Já foi feito pelos colegas (não entrar em retrabalho)

- JWT configurado em Aluno API e Pagamentos API.
- `[ClaimsAuthorize]` ativo em `CursoController`, `AulaController`, `FaturamentoController`.
- Migration + seed automáticos em Aluno API e Pagamentos API.
- `appsettings.json` da Pagamentos API com `JwtSettings` e `MessageQueueConnection`.
- Classe `PagamentoRecusadoEvent` criada (mas sem publicação/consumo — ver item 3).
- Propriedade `PagamentoPodeSerRealizado` no command (mas sem validação — ver item 7).
- `ConcluirCursoCommandHandler` consultando `AulasFaltantes` (mas depende de 5).
- `[AllowAnonymous]` removido do `AdminController` do BFF (mas login manual continua — ver item 9).
