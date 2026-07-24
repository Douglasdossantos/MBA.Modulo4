# Simula localmente a esteira do .github/workflows/ci.yml
# (restore -> lint -> build Release -> testes+cobertura -> relatorio -> gate -> vuln).
# Uso:  .\scripts\ci-local.ps1        (execute na raiz do repositorio)
$ErrorActionPreference = "Stop"
$sln = "MBA.Modulo4.sln"
$MIN_LINE = 42   # mesmo valor do gate no ci.yml; suba conforme a cobertura crescer (criterio: 80)

Write-Host "==> Restore" -ForegroundColor Cyan
dotnet restore $sln

Write-Host "==> Lint (dotnet format --verify-no-changes) [advisory]" -ForegroundColor Cyan
dotnet format $sln --verify-no-changes --severity warn --no-restore
if ($LASTEXITCODE -ne 0) { Write-Warning "dotnet format encontrou desvios (advisory no CI; rode 'dotnet format' para corrigir)." }

Write-Host "==> Build (Release)" -ForegroundColor Cyan
dotnet build $sln --configuration Release --no-restore

Write-Host "==> Testes + cobertura" -ForegroundColor Cyan
Remove-Item -Recurse -Force TestResults, coveragereport -ErrorAction SilentlyContinue
dotnet test $sln --configuration Release --no-build --settings coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory ./TestResults --verbosity normal

Write-Host "==> Relatorio de cobertura" -ForegroundColor Cyan
if (-not (Get-Command reportgenerator -ErrorAction SilentlyContinue)) {
  dotnet tool install --global dotnet-reportgenerator-globaltool --add-source https://api.nuget.org/v3/index.json --ignore-failed-sources | Out-Null
}
reportgenerator -reports:"TestResults/**/coverage.cobertura.xml" -targetdir:coveragereport -reporttypes:"Cobertura;TextSummary"
Get-Content coveragereport/Summary.txt | Write-Host

# Gate por LINHA (nao branch): em codigo async o coverlet conta os ramos ocultos das maquinas
# de estado de async/await, poluindo o branch coverage. Cobertura de linha e a metrica estavel.
Write-Host "==> Gate de cobertura (linha >= $MIN_LINE%)" -ForegroundColor Cyan
[xml]$cov = Get-Content coveragereport/Cobertura.xml
$pct = [math]::Round(([double]$cov.coverage.'line-rate') * 100, 1)
Write-Host "Line coverage: $pct% (minimo $MIN_LINE%)"
if ($pct -lt $MIN_LINE) { Write-Error "FALHOU: cobertura de linha $pct% abaixo do minimo $MIN_LINE%"; exit 1 }

Write-Host "==> Dependencias vulneraveis [advisory]" -ForegroundColor Cyan
dotnet list $sln package --vulnerable --include-transitive

Write-Host ""
Write-Host "OK - esteira local passou (equivalente ao ci.yml)" -ForegroundColor Green
