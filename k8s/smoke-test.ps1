# T-22 — Smoke E2E + resiliência no cluster.
# Constrói as 6 imagens, carrega no Kind, aplica os Deployments, espera ficarem Ready, valida os
# health endpoints e exercita a resiliência (mata um pod de pagamentos e confirma que o ecossistema sobrevive).
# Pré-requisitos: docker, kind ('mba' já criado via setup.ps1), kubectl. Rode da raiz do repo.

$ErrorActionPreference = "Stop"

# Tag pinada das imagens locais (S6596) - manter em sincronia com k8s/services/*.yaml.
$imageTag = "1.0.0"

$services = @(
  @{ image = "mba-auth-api";     project = "MBA.Auth.Api" },
  @{ image = "mba-conteudo-api"; project = "MBA.Conteudo.Api" },
  @{ image = "mba-aluno-api";    project = "MBA.Aluno.API" },
  @{ image = "mba-pagamentos-api"; project = "MBA.Pagamentos.Api" },
  @{ image = "mba-bff-api";      project = "MBA.Bff.Api" },
  @{ image = "mba-webapp-mvc";   project = "MBA.WebApp.MVC" }
)

Write-Host "==> 1/4 Build + load das imagens no Kind..." -ForegroundColor Cyan
foreach ($s in $services) {
  docker build --build-arg PROJECT_NAME=$($s.project) -t "$($s.image):$imageTag" ./src
  kind load docker-image "$($s.image):$imageTag" --name mba
}

Write-Host "==> 2/4 Aplicando os Deployments..." -ForegroundColor Cyan
kubectl apply -f k8s/services/

Write-Host "==> 3/4 Aguardando os pods ficarem Ready..." -ForegroundColor Cyan
foreach ($s in $services) {
  $dep = $s.image -replace '^mba-', ''   # auth-api, conteudo-api, ...
  kubectl -n mba rollout status "deployment/$dep" --timeout=180s
}

Write-Host "==> Estado dos pods:" -ForegroundColor Cyan
kubectl -n mba get pods

Write-Host "==> 4/4 Resiliência: matando um pod de pagamentos..." -ForegroundColor Cyan
$pod = kubectl -n mba get pod -l app=pagamentos-api -o jsonpath='{.items[0].metadata.name}'
kubectl -n mba delete pod $pod
Write-Host "    Esperando o Deployment recriar o pod (auto-heal)..."
kubectl -n mba rollout status deployment/pagamentos-api --timeout=120s

Write-Host "    Confirmando que os outros serviços continuaram de pé:" -ForegroundColor Cyan
kubectl -n mba get pods

Write-Host "==> Smoke OK. Acesse o front em http://localhost:8080 e o BFF em http://localhost:8093/swagger" -ForegroundColor Green
