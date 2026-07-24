# Execução de testes, cobertura e análise estática — Módulo 5

Scripts para reproduzir localmente a mesma esteira que roda no GitHub Actions.

## Pré-requisitos

- **.NET 8 SDK** (todos os projetos são `net8.0`).
- PowerShell (Windows).
- Para a análise SonarCloud (opcional): **Java 17+** e as ferramentas globais
  `dotnet-sonarscanner` e `dotnet-coverage` (o `sonar-local.ps1` cuida do Java).

> Observação sobre NuGet: se a máquina tiver um feed privado (ex.: Grial) que retorne
> 401, instale as ferramentas fixando o nuget.org:
> `dotnet tool install --global <tool> --add-source https://api.nuget.org/v3/index.json --ignore-failed-sources`

## 1. Testes + cobertura (espelho do `ci.yml`)

```powershell
.\scripts\ci-local.ps1
```

O script executa, na ordem:

1. `dotnet restore`
2. `dotnet format --verify-no-changes` (lint — **advisory**, não quebra)
3. `dotnet build -c Release`
4. `dotnet test` com cobertura via `coverlet` (`--settings coverlet.runsettings`)
5. Geração do relatório com **ReportGenerator**
6. **Gate de cobertura de LINHA** (mínimo definido em `$MIN_LINE`, hoje **42%**)
7. `dotnet list package --vulnerable` (advisory)

Estado atual: **240 testes** (237 passam, 3 skips de smoke), cobertura de **linha ~45%**.

### Por que o gate é por LINHA e não por branch?

Em código `async`, o coverlet conta os ramos ocultos das máquinas de estado de
`async/await`, o que infla e distorce o *branch coverage*. A cobertura de **linha** é a
métrica estável e comparável entre serviços — por isso o gate usa linha. Detalhe
documentado no cabeçalho do `coverlet.runsettings`.

## 2. Análise estática (SonarCloud)

```powershell
$env:SONAR_TOKEN = "seu-token"
.\scripts\sonar-local.ps1 -Org "sua-org-key" -Key "sua-project-key"
```

O script resolve o Java 17 automaticamente e roda `begin → build → dotnet-coverage → end`,
enviando o resultado para o SonarCloud. Parâmetros:

- `-Org` — Organization Key (ex.: `geraldsimon`), tirada da URL
  `sonarcloud.io/organizations/<org>`.
- `-Key` — Project Key do projeto no SonarCloud.
- `-HostUrl` — padrão `https://sonarcloud.io` (região EU). Para a região US, passe
  `-HostUrl "https://sonarqube.us"`.

Pré-requisitos no SonarCloud: criar organização + projeto (pode ser "Create a project
manually"), **desligar o Automatic Analysis** (Administration → Analysis Method) e gerar
um token (My Account → Security). O resultado abre em `sonarcloud.io/dashboard?id=<key>`.

## 3. Equivalência com o CI

`ci-local.ps1` reproduz o `.github/workflows/ci.yml`; `sonar-local.ps1` reproduz o
`.github/workflows/sonarcloud.yml`. No CI, o job do SonarCloud só roda se o secret
`SONAR_TOKEN` existir no repositório — caso contrário ele se auto-pula e o CI segue verde.
