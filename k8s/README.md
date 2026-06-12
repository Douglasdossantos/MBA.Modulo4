# Kubernetes — MBA Módulo 4 (Plataforma Educacional)

Orquestração dos microsserviços em Kubernetes (Kind local). Esta pasta tem a **base do cluster** (T-15);
os Deployments por serviço entram em seguida (T-16–T-21).

## Estrutura

```
k8s/
├── namespace.yaml        # namespace "mba"
├── kind-config.yaml      # cluster Kind (1 nó) + port mappings p/ o host
├── setup.ps1             # cria o cluster e aplica a base
├── base/
│   ├── configmap.yaml    # config compartilhada (env, RabbitMQ, URLs internas, DATABASE_PROVIDER)
│   └── secret.yaml       # segredos DEMO (JWT, senha do SA) — não usar em produção
└── infra/
    ├── rabbitmq.yaml     # broker (Deployment + Service)
    └── sqlserver.yaml    # SQL Server (Deployment + Service + PVC) — opcional/pesado
```

## Pré-requisitos
`docker`, `kind` e `kubectl` no PATH.

## Subir a base
```powershell
# da raiz do repo
pwsh k8s/setup.ps1
```
Ou manualmente:
```bash
kind create cluster --name mba --config k8s/kind-config.yaml
kubectl apply -f k8s/namespace.yaml
kubectl apply -f k8s/base/
kubectl apply -f k8s/infra/
kubectl -n mba get pods
```

## Banco de dados
Por padrão os serviços rodam em **SQLite** (`mba-config: DATABASE_PROVIDER=Sqlite`) — leve e confiável no
Kind. O **SQL Server** está implantado (requisito do PDF) e pode ser usado trocando
`DATABASE_PROVIDER=SqlServer` na ConfigMap e apontando as connection strings dos serviços para
`sqlserver:1433` (usuário `sa`, senha no Secret `mba-secrets/SA_PASSWORD`). ⚠️ o mssql pede ~2GB de RAM.

## Probes
Cada serviço expõe `/health/live` (liveness) e `/health/ready` (readiness) — usados pelos
`livenessProbe`/`readinessProbe` dos Deployments por serviço.

## Próximo
Aplicar `k8s/services/` (T-16–T-21): Deployment + Service por API/front, referenciando `mba-config` e
`mba-secrets`, com as imagens publicadas no Docker Hub (T-05).
```
kubectl apply -f k8s/services/
```
