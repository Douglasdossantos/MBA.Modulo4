# Dockerfile dedicado — Aluno API. Contexto de build: ./src
#   docker build -f src/MBA.Aluno.API/Dockerfile -t mba-aluno-api:latest ./src
# Equivalente ao src/Dockerfile parametrizado, com o projeto fixo neste serviço.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
# curl: healthcheck do compose. /app/data: pasta do SQLite.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /app/data

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "MBA.Aluno.API/MBA.Aluno.API.csproj"
RUN dotnet publish "MBA.Aluno.API/MBA.Aluno.API.csproj" \
    -c Release -o /app/publish \
    /p:UseAppHost=false /p:ErrorOnDuplicatePublishOutputFiles=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MBA.Aluno.API.dll"]
