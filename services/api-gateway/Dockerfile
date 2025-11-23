FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["services/api-gateway/src/ApiGateway.API/ApiGateway.API.csproj", "services/api-gateway/src/ApiGateway.API/"]

RUN dotnet restore "services/api-gateway/src/ApiGateway.API/ApiGateway.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/services/api-gateway/src/ApiGateway.API"
RUN dotnet build "ApiGateway.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ApiGateway.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.API.dll"]