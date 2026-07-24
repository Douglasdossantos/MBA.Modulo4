# Roda a analise do SonarCloud localmente — equivalente ao .github/workflows/sonarcloud.yml.
# Uso (com valores REAIS do seu projeto, nao os placeholders):
#   $env:SONAR_TOKEN = "seu-token-do-sonarcloud"
#   .\scripts\sonar-local.ps1 -Org "sua-org-key-real" -Key "sua-project-key-real"
#
# Regiao: -HostUrl padrao e a EU (https://sonarcloud.io). Para a regiao US, passe
#   -HostUrl "https://sonarqube.us".
param(
	[Parameter(Mandatory = $true)] [string]$Org,
	[Parameter(Mandatory = $true)] [string]$Key,
	[string]$Token = $env:SONAR_TOKEN,
	[string]$HostUrl = "https://sonarcloud.io",
	[string]$Sln = "MBA.Modulo4.sln"
)
$ErrorActionPreference = "Stop"

function Assert-Ok($passo) {
	if ($LASTEXITCODE -ne 0) {
		Write-Error "FALHOU em '$passo' (exit code $LASTEXITCODE). Abortando."
		exit 1
	}
}

if ([string]::IsNullOrWhiteSpace($Token) -or $Token -eq "cole-seu-token") {
	Write-Error "Token ausente ou placeholder. Defina o real:  `$env:SONAR_TOKEN = '...'"
	exit 1
}
if ($Org -like "sua-*" -or $Key -like "sua-*") {
	Write-Error "Org/Key ainda sao placeholders. Use os valores reais do seu projeto no SonarCloud."
	exit 1
}

# O scanner precisa de Java 11+; nao confia no 'java' do PATH (que pode ser o 8).
$jdk = Get-ChildItem 'C:\Program Files\Microsoft\jdk-17*' -Directory -ErrorAction SilentlyContinue | Select-Object -First 1
if ($jdk) {
	$env:JAVA_HOME = $jdk.FullName
	$env:Path = "$($jdk.FullName)\bin;$env:Path"
}
Write-Host "==> Java em uso:" -ForegroundColor Cyan
java -version

$exclusions = "**/bin/**,**/obj/**,**/.publish/**,**/wwwroot/lib/**,**/wwwroot/assets/**,**/wwwroot/css/**,**/Migrations/**,**/*.Designer.cs,**/GlobalUsings.cs,**/appsettings.Testing.json"

Write-Host "==> Sonar Begin (host: $HostUrl)" -ForegroundColor Cyan
dotnet sonarscanner begin `
	/k:"$Key" `
	/o:"$Org" `
	/d:sonar.host.url="$HostUrl" `
	/d:sonar.token="$Token" `
	/d:sonar.scm.disabled=true `
	/d:sonar.exclusions="$exclusions" `
	/d:sonar.cs.vscoveragexml.reportsPaths="coverage.xml" `
	/d:sonar.sourceEncoding=UTF-8
Assert-Ok "sonar begin"

Write-Host "==> Build (Release)" -ForegroundColor Cyan
dotnet build $Sln --configuration Release --no-incremental
Assert-Ok "build"

Write-Host "==> Testes + cobertura (dotnet-coverage)" -ForegroundColor Cyan
dotnet-coverage collect "dotnet test $Sln --configuration Release --no-build" -f xml -o coverage.xml
Assert-Ok "testes + cobertura"

Write-Host "==> Sonar End (envia para o SonarCloud)" -ForegroundColor Cyan
dotnet sonarscanner end /d:sonar.token="$Token"
Assert-Ok "sonar end"

Write-Host ""
Write-Host "OK - analise enviada. Veja o resultado em $HostUrl -> seu projeto." -ForegroundColor Green
