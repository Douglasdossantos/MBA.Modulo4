# Dockerfile dedicado — Auth API. Contexto de build: ./src
#   docker build -f src/MBA.Auth.Api/Dockerfile -t mba-auth-api:1.0.0 ./src
# Equivalente ao src/Dockerfile parametrizado, com o projeto fixo neste serviço.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
# curl: healthcheck do compose. /app/data: pasta do SQLite.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /app/data \
    && chown -R app:app /app/data

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "MBA.Auth.Api/MBA.Auth.Api.csproj"
RUN dotnet publish "MBA.Auth.Api/MBA.Auth.Api.csproj" \
    -c Release -o /app/publish \
    /p:UseAppHost=false /p:ErrorOnDuplicatePublishOutputFiles=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Executa como o usuário não-root 'app' da imagem base (hotspot S6471 do SonarQube).
USER app

ENTRYPOINT ["dotnet", "MBA.Auth.Api.dll"]
