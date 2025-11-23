# Alert Service Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy shared libraries first
COPY shared/ shared/

# Copy service source
COPY services/alert-service/src/ services/alert-service/src/

# Restore dependencies
RUN dotnet restore "services/alert-service/src/AlertService.API/AlertService.API.csproj"

# Build
RUN dotnet build "services/alert-service/src/AlertService.API/AlertService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "services/alert-service/src/AlertService.API/AlertService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlertService.API.dll"]