# Dockerfile dedicado — BFF API. Contexto de build: ./src
#   docker build -f src/MBA.Bff.Api/Dockerfile -t mba-bff-api:latest ./src
# Equivalente ao src/Dockerfile parametrizado, com o projeto fixo neste serviço.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
# curl: healthcheck do compose.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /app/data

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "MBA.Bff.Api/MBA.Bff.Api.csproj"
RUN dotnet publish "MBA.Bff.Api/MBA.Bff.Api.csproj" \
    -c Release -o /app/publish \
    /p:UseAppHost=false /p:ErrorOnDuplicatePublishOutputFiles=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MBA.Bff.Api.dll"]
