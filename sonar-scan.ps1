param(
    [Parameter(Mandatory=$false)]
    [string]$Token,
    [string]$Server = "http://localhost:9000",
    [string]$Key    = "mba-modulo4"
)

if (-not $Token) {
    $Token = $env:SONAR_TOKEN_MBA_MODULO4
}

$ProjectPath = "MBA.Modulo4.sln"

if (-not $Token) {
    Write-Error "Token nao encontrado. Passe -Token <valor> ou defina SONAR_TOKEN_MBA_MODULO4."
    exit 1
}

if (-not (Get-Command dotnet-sonarscanner -ErrorAction SilentlyContinue)) {
    Write-Host "Instalando dotnet-sonarscanner..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-sonarscanner
}

Write-Host ""
Write-Host "Iniciando analise SonarQube - $Key" -ForegroundColor Cyan
Write-Host "   Servidor : $Server"
Write-Host "   Projeto  : $ProjectPath"
Write-Host ""

dotnet sonarscanner begin /k:"$Key" /d:sonar.host.url="$Server" /d:sonar.token="$Token" /d:sonar.scm.disabled=true /d:sonar.exclusions="**/bin/**,**/obj/**,**/.publish/**,**/wwwroot/lib/**,**/wwwroot/assets/libs/**,**/wwwroot/assets/css/**,**/wwwroot/assets/scss/**,**/wwwroot/assets/icon-fonts/**,**/wwwroot/css/**,**/Migrations/**,**/*.Designer.cs,**/GlobalUsings.cs,**/tests/**,**/appsettings.Testing.json" /d:sonar.sourceEncoding=UTF-8

if ($LASTEXITCODE -ne 0) { Write-Error "Falha no Begin."; exit 1 }

Write-Host ""
Write-Host "Build..." -ForegroundColor Cyan
dotnet build $ProjectPath --configuration Release --no-incremental

if ($LASTEXITCODE -ne 0) { Write-Error "Falha no Build."; exit 1 }

Write-Host ""
Write-Host "Enviando resultados..." -ForegroundColor Cyan
dotnet sonarscanner end /d:sonar.token="$Token"

if ($LASTEXITCODE -ne 0) { Write-Error "Falha no End."; exit 1 }

Write-Host ""
Write-Host "Analise concluida! Acesse: $Server/dashboard?id=$Key" -ForegroundColor Green
