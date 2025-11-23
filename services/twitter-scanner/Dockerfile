# Twitter Scanner Service Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy shared libraries first
COPY shared/ shared/

# Copy service source
COPY services/twitter-scanner/src/ services/twitter-scanner/src/

# Restore dependencies
RUN dotnet restore "services/twitter-scanner/src/TwitterScanner.API/TwitterScanner.API.csproj"

# Build
RUN dotnet build "services/twitter-scanner/src/TwitterScanner.API/TwitterScanner.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "services/twitter-scanner/src/TwitterScanner.API/TwitterScanner.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitterScanner.API.dll"]