# ============================================================
# Multi-stage Dockerfile for all MBA.Modulo4 services.
# Usage: docker build --build-arg PROJECT_NAME=MBA.Auth.Api .
# ============================================================

# --- Runtime base image ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# curl: usado pelo healthcheck do docker-compose (a imagem aspnet não traz). + pasta de dados do SQLite.
# O chown permite que o usuário não-root 'app' (embutido na imagem aspnet) escreva os bancos SQLite;
# volumes nomeados herdam essa permissão na primeira utilização.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /app/data \
    && chown -R app:app /app/data

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG PROJECT_NAME
WORKDIR /src

# 1) Copy all .csproj files first (layer caching for restore)
COPY MBA.Core/MBA.Core.csproj MBA.Core/
COPY MBA.MessageBus/MBA.MessageBus.csproj MBA.MessageBus/
COPY MBA.WebApi.Core/MBA.WebApi.Core.csproj MBA.WebApi.Core/

COPY MBA.Auth.Api/MBA.Auth.Api.csproj MBA.Auth.Api/

COPY MBA.Conteudo.Domain/MBA.Conteudo.Domain.csproj MBA.Conteudo.Domain/
COPY MBA.Conteudo.Data/MBA.Conteudo.Data.csproj MBA.Conteudo.Data/
COPY MBA.Conteudo.Application/MBA.Conteudo.Application.csproj MBA.Conteudo.Application/
COPY MBA.Conteudo.Api/MBA.Conteudo.Api.csproj MBA.Conteudo.Api/

COPY MBA.Aluno.Domain/MBA.Aluno.Domain.csproj MBA.Aluno.Domain/
COPY MBA.Aluno.Data/MBA.Aluno.Data.csproj MBA.Aluno.Data/
COPY MBA.Aluno.Application/MBA.Aluno.Application.csproj MBA.Aluno.Application/
COPY MBA.Aluno.API/MBA.Aluno.API.csproj MBA.Aluno.API/

COPY MBA.Pagamentos.Domain/MBA.Pagamentos.Domain.csproj MBA.Pagamentos.Domain/
COPY MBA.Pagamentos.Data/MBA.Pagamentos.Data.csproj MBA.Pagamentos.Data/
COPY MBA.Pagamentos.Application/MBA.Pagamentos.Application.csproj MBA.Pagamentos.Application/
COPY MBA.Pagamentos.Api/MBA.Pagamentos.Api.csproj MBA.Pagamentos.Api/

COPY MBA.Bff.Api/MBA.Bff.Api.csproj MBA.Bff.Api/
COPY MBA.WebApp.MVC/MBA.WebApp.MVC.csproj MBA.WebApp.MVC/

# 2) Restore only the target project (pulls transitive deps)
RUN dotnet restore "${PROJECT_NAME}/${PROJECT_NAME}.csproj"

# 3) Copy all source code
COPY . .

# 4) Publish
RUN dotnet publish "${PROJECT_NAME}/${PROJECT_NAME}.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:ErrorOnDuplicatePublishOutputFiles=false

# --- Final image ---
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Executa como o usuário não-root 'app' da imagem base (hotspot S6471 do SonarQube).
# A porta 8080 não exige privilégio de root.
USER app

# The ENTRYPOINT + CMD will be set via docker-compose
ENTRYPOINT ["dotnet"]
