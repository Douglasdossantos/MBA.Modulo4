# Sobe o cluster Kind e aplica a BASE (namespace + config/secret + infra RabbitMQ/SQL Server).
# Os Deployments dos serviços (T-16+) entram depois, com as imagens já publicadas no Docker Hub (T-05).
# Pré-requisitos: docker, kind, kubectl no PATH.
#
# Uso:  pwsh k8s/setup.ps1   (rode da raiz do repo)

$ErrorActionPreference = "Stop"

Write-Host "==> Criando cluster Kind 'mba'..." -ForegroundColor Cyan
kind create cluster --name mba --config k8s/kind-config.yaml

Write-Host "==> Namespace + config + secrets..." -ForegroundColor Cyan
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/base/

Write-Host "==> Infra (RabbitMQ + SQL Server)..." -ForegroundColor Cyan
kubectl apply -f k8s/infra/

Write-Host "==> Aguardando a infra ficar pronta..." -ForegroundColor Cyan
kubectl -n mba rollout status deployment/rabbitmq --timeout=180s
# SQL Server é pesado; não bloqueia o setup se demorar (os serviços rodam em SQLite por padrão).
kubectl -n mba rollout status deployment/sqlserver --timeout=240s 2>$null

Write-Host "==> Base pronta. Próximo: aplicar os Deployments dos serviços (k8s/services/)." -ForegroundColor Green
kubectl -n mba get pods
