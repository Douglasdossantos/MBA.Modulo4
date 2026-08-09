# Dockerfile dedicado — WebApp MVC (front). Contexto de build: ./src
#   docker build -f src/MBA.WebApp.MVC/Dockerfile -t mba-webapp-mvc:1.0.0 ./src
# Equivalente ao src/Dockerfile parametrizado, com o projeto fixo neste serviço.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
# curl: healthcheck do compose.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /app/data \
    && chown -R app:app /app/data

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "MBA.WebApp.MVC/MBA.WebApp.MVC.csproj"
RUN dotnet publish "MBA.WebApp.MVC/MBA.WebApp.MVC.csproj" \
    -c Release -o /app/publish \
    /p:UseAppHost=false /p:ErrorOnDuplicatePublishOutputFiles=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Executa como o usuário não-root 'app' da imagem base (hotspot S6471 do SonarQube).
USER app

ENTRYPOINT ["dotnet", "MBA.WebApp.MVC.dll"]
