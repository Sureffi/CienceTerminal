# Token Metrics Service Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy shared libraries first
COPY shared/ shared/

# Copy service source
COPY services/token-metrics-service/src/ services/token-metrics-service/src/

# Restore dependencies
RUN dotnet restore "services/token-metrics-service/src/TokenMetrics.API/TokenMetrics.API.csproj"

# Build
RUN dotnet build "services/token-metrics-service/src/TokenMetrics.API/TokenMetrics.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "services/token-metrics-service/src/TokenMetrics.API/TokenMetrics.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TokenMetrics.API.dll"]
