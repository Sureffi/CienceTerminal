# Build React app
FROM node:18-alpine AS react-build
WORKDIR /app
COPY src/frontend/package*.json ./
RUN npm ci
COPY src/frontend/ ./
RUN npm run build

# Build .NET API
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dotnet-build
WORKDIR /app
COPY CienceTerminal.sln ./
COPY src/backend/CienceTerminal.API/*.csproj ./src/backend/CienceTerminal.API/
COPY src/backend/CienceTerminal.Application/*.csproj ./src/backend/CienceTerminal.Application/
COPY src/backend/CienceTerminal.Core/*.csproj ./src/backend/CienceTerminal.Core/
COPY src/backend/CienceTerminal.Infrastructure/*.csproj ./src/backend/CienceTerminal.Infrastructure/
COPY src/backend/CienceTerminal.Shared/*.csproj ./src/backend/CienceTerminal.Shared/
RUN dotnet restore
COPY src/backend/ ./src/backend/
RUN dotnet publish src/backend/CienceTerminal.API -c Release -o out

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=dotnet-build /app/out .
COPY --from=react-build /app/dist ./wwwroot/
# Render uses PORT environment variable, default to 10000
EXPOSE $PORT
ENV PORT=10000
ENTRYPOINT ["dotnet", "CienceTerminal.API.dll"]
