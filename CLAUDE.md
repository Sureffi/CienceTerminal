# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CienceTerminal is a cryptocurrency monitoring and alert system focused on real-time Twitter monitoring for Contract Address (CA) mentions and crypto-related content. The system follows a **microservices architecture** with event-driven communication using AWS SNS/SQS, featuring tier-based access control and Auth0 authentication.

## Architecture

The project follows **microservices architecture** with event-driven communication:

```
Frontend (React 19) → API Gateway → Services
                          ↓
    Twitter Scanner ──SNS/SQS──→ Alert Service ──SignalR──→ Clients
          ↓                            ↓
    AI Classification          Token Metrics Service
```

### Core Services:
- **API Gateway** (`services/api-gateway/`): Single entry point with Auth0 JWT authentication, routes to backend services
- **Twitter Scanner Service** (`services/twitter-scanner/`): Twitter stream ingestion, Groq AI classification, and CA detection
- **Alert Service** (`services/alert-service/`): Alert processing, SQS message consumption, SignalR distribution with tier-based delays
- **Token Metrics Service** (`services/token-metrics-service/`): Token data aggregation, trending analysis, and metrics tracking
- **Shared Libraries** (`shared/`): Event contracts (`CienceTerminal.Contracts`), AWS integration (`CienceTerminal.AWS`), and common utilities (`CienceTerminal.Common`)
- **Frontend** (`frontend/`): React 19 + TypeScript SPA with Auth0 authentication, SignalR real-time updates, and tier-based UI

Each service follows **Clean Architecture** with Domain, Application, Infrastructure, and API layers.

## Common Development Commands

### Quick Start Scripts
```bash
# Start development environment (hybrid mode: LocalStack + native services)
./scripts/dev-start.sh

# Start full Docker stack (everything containerized)
./scripts/dev-start.sh --docker

# Start backend in Docker, frontend native (hot reload for UI dev)
./scripts/dev-start.sh --frontend-only

# Stop development environment
./scripts/dev-stop.sh

# View logs from all services
./scripts/dev-logs.sh

# Run all tests
./scripts/dev-test.sh
```

### Microservices (.NET 8)
```bash
# Build entire solution
dotnet build

# Run specific services
dotnet run --project services/api-gateway/src/ApiGateway.API
dotnet run --project services/twitter-scanner/src/TwitterScanner.API
dotnet run --project services/alert-service/src/AlertService.API
dotnet run --project services/token-metrics-service/src/TokenMetrics.API

# Build specific service
dotnet build services/twitter-scanner/src/TwitterScanner.API

# Run tests (note: test projects not yet implemented)
dotnet test

# Watch mode for specific service during development
dotnet watch --project services/twitter-scanner/src/TwitterScanner.API
```

### Frontend (React + TypeScript)
```bash
cd frontend

# Install dependencies
npm install

# Development server with hot reload
npm run dev

# Production build
npm run build

# Lint code
npm run lint

# Build and copy to API wwwroot (legacy integration)
npm run build-to-api
```

### Docker Operations
```bash
# Full stack with all services
docker-compose up

# LocalStack only (for hybrid development)
docker-compose up -d localstack

# Specific service
docker-compose up -d twitter-scanner

# View logs
docker-compose logs -f twitter-scanner
docker-compose logs -f alert-service

# Rebuild after code changes
docker-compose up --build

# Clean everything
docker-compose down -v
```

### AWS LocalStack (Development)
```bash
# Setup all SNS topics and SQS queues (run after starting LocalStack)
./scripts/setup-aws-resources-docker.sh

# Check LocalStack health
curl http://localhost:4566/_localstack/health

# List SNS topics
aws --endpoint-url=http://localhost:4566 sns list-topics

# List SQS queues
aws --endpoint-url=http://localhost:4566 sqs list-queues

# Receive messages from queue (useful for debugging)
aws --endpoint-url=http://localhost:4566 sqs receive-message --queue-url http://localhost:4566/000000000000/alert-service-twitter-alerts-queue
```

## Database Schema

**Shared Database**: `cienceterminal` (PostgreSQL) - accessed by both Alert Service and Token Metrics Service

### Token Metrics Service Database (`TokenMetricsDbContext`)

**`coins` table** (Token Metrics owns all writes, Alert Service read-only):
- `Id` (uuid, PK)
- `CoinMintAddress` (varchar(44), unique, indexed) - Solana contract address
- `CoinSymbol` (varchar(50), nullable) - Token symbol from Jupiter
- `CoinImage` (varchar(500), nullable) - Logo URL from Jupiter
- `MentionCount24h` (int, default 0, indexed desc where IsActive=true) - Denormalized from aggregates
- `HolderCount` (int, nullable) - From Helius RPC
- `Liquidity` (numeric(28,8), nullable) - Total liquidity USD from Jupiter
- `Volume24h` (numeric(28,8), nullable) - 24h volume USD from Jupiter
- `MarketCap` (numeric(28,8), nullable) - Market cap USD from Jupiter
- `TopHoldersPercentage` (numeric(5,2), nullable) - Supply % by top holders
- `FirstPoolCreatedAt` (timestamptz, nullable) - Token launch date
- `LastUpdated` (timestamptz, indexed) - Last metadata refresh
- `IsActive` (bool, default true) - Legacy field, not used
- `IsBlacklisted` (bool, default false) - Scam/rug flag

**`mention_aggregates` table** (Token Metrics owns, Alert Service read-only):
- `Id` (uuid, PK)
- `CoinMintAddress` (varchar(44), unique, indexed)
- `MentionCount5m` (double, default 0.0) - Rolling 5-minute count
- `MentionCount1h` (double, default 0.0) - Rolling 1-hour count
- `MentionCount6h` (double, default 0.0) - Rolling 6-hour count
- `MentionCount24h` (double, default 0.0) - Rolling 24-hour count
- `TrendingScore` (double, default 0.0, indexed desc) - EMA-based momentum
- `Rank` (int, nullable, indexed where not null) - Position in top 25 (1-25)
- `LastMentioned` (timestamptz) - Most recent mention
- `LastCalculated` (timestamptz) - When aggregates were computed

**`ca_mention_records` table** (Token Metrics owns):
- `Id` (uuid, PK)
- `CoinMintAddress` (varchar(44), indexed with Timestamp)
- `TweetId` (varchar(20), unique composite with CoinMintAddress + AuthorId)
- `AuthorId` (varchar(20))
- `Username` (varchar(100))
- `ProfilePicture` (varchar(500))
- `Followers` (int)
- `IsVerified` (bool)
- `TweetContent` (varchar(30000), nullable)
- `TweetUrl` (varchar(200))
- `IsOriginalPost`, `IsReply`, `IsRetweet`, `IsQuote` (bool)
- `Timestamp` (timestamptz)

### Alert Service Database (`AlertServiceDbContext`)

**`alerts` table** (Alert Service owns):
- `Id` (uuid, PK)
- `AlertType` (varchar(50), indexed) - Discriminator for polymorphic alerts
- `AlertData` (jsonb) - Full alert JSON payload
- `CoinMintAddress` (varchar(44), nullable, indexed) - For coin-related alerts
- `CreatedAt` (timestamptz, indexed)

### Database Access Pattern
- **Alert Service**:
  - Reads `coins` and `mention_aggregates` via `TokenMetricsReadOnlyDbContext` (read-only)
  - Full write access to `alerts` table via `AlertServiceDbContext`
- **Token Metrics Service**:
  - Full ownership of `coins`, `mention_aggregates`, `ca_mention_records`
  - Reads `alerts` table to determine which coins need metrics updates (read-only)

## Key System Components

### Core Domain Entities
- **Alert**: Abstract base class for all alert types (`TwitterAlert`, `CaMentionAlert`) with polymorphic JSON serialization
- **CaMention**: Cryptocurrency contract address mentions with tracking state (new, accelerating, declining, dying)
- **CaMentionTracking**: Tracks CA mention velocity and engagement over time for trend detection
- **Tweet**: Twitter data model with author, engagement metrics, and CA extraction
- **Author**: Twitter user profile information and verification status
- **TokenMetadata**: Solana token information from Jupiter aggregator and Helius RPC

### Critical Infrastructure
- **IngestionService** (Twitter Scanner): Background service consuming Twitter WebSocket streams, orchestrates AI classification
- **SqsEventConsumer** (Alert Service): Generic background service consuming SQS messages and processing via MediatR
- **SignalR Hubs**: Tier-based real-time distribution with Auth0 JWT authentication and user grouping
- **MetadataService**: Jupiter aggregator and Helius RPC integration for Solana token data

### Event-Driven Communication
**SNS Topics** (defined in `CienceTerminal.AWS/Constants/AwsTopics.cs`):
- `cienceterminal-twitter-alerts`: General Twitter alerts from scanner
- `cienceterminal-ca-mention-alerts`: Contract address mention alerts with trend analysis
- `cienceterminal-token-trending-list`: Updated trending token lists
- `cienceterminal-token-metrics-updated`: Token metrics changes

**SQS Queues** (subscribed to SNS topics):
- `alert-service-twitter-alerts-queue`: Consumed by Alert Service
- `alert-service-ca-mention-alerts-queue`: Consumed by Alert Service
- `alert-service-alert-removal-queue`: Handles alert expiration/removal
- `alert-service-token-metrics-updated-queue`: Consumed by Alert Service for coin metrics updates
- `token-metrics-service-ca-mention-detected-queue`: Consumed by Token Metrics Service

**CQRS Pattern**: MediatR commands/queries within service boundaries only; cross-service via SNS/SQS events

## Data Flow Architecture

```
1. Tweet Ingestion & Classification:
   Twitter API → WebSocket → IngestionService (Twitter Scanner)
                                    ↓
                              Groq AI Classification
                                    ↓
                         Extract CAs + Analyze Trends
                                    ↓
                    Publish Events to SNS Topics

2. Alert Distribution:
   SNS Topics → SQS Queues → SqsEventConsumer (Alert Service)
                                    ↓
                            Process via MediatR
                                    ↓
                    Distribute via SignalR Hubs with tier delays
                                    ↓
                            Frontend Clients

3. Token Metrics Updates:
   Token Metrics Service (background job every 60s):
   1. Query alerts table for coins with active alerts
   2. Fetch fresh data from Jupiter/Helius APIs
   3. Update coins table with latest metrics
   4. Publish TokenMetricsUpdatedEvent to SNS
                              ↓
                    Alert Service consumes event
                              ↓
                    Query fresh coin data from coins table
                              ↓
                    Update active alerts via AlertManager
                              ↓
                    Push updates to frontend via SignalR
```

**Key Flows**:
- **Twitter Scanner → Alert Service**: Asynchronous via SNS/SQS (decoupled)
- **Alert Service → Frontend**: Real-time via SignalR with tier-based groups
- **Authentication**: Auth0 JWT tokens validated at API Gateway and SignalR connection
- **Tier-Based Access**: Free tier gets delayed alerts, premium gets instant alerts

## Configuration & Environment

### Configuration Pattern
- **Single `.env` file** at repository root contains all configuration (shared across services)
- **DependencyInjection.cs** files in each layer register services via extension methods
- **Options pattern** with strongly-typed configuration classes (e.g., `ApiGatewayOptions`, `ServiceOptions`)
- **DotNetEnv** library loads `.env` file in `Program.cs` of each service
- Services override container-specific values via docker-compose environment variables

### Environment Variables (see `.env`)
**Required API Keys**:
- `ApiKeys__Groq`: Groq AI API key for tweet classification
- `ApiKeys__Twitter`: Twitter API key for WebSocket stream
- `ApiKeys__Helius`: Helius RPC API key for Solana blockchain data

**Auth0 Configuration**:
- `AUTH0_DOMAIN`: Auth0 tenant domain
- `AUTH0_AUDIENCE`: API identifier for JWT validation
- `VITE_AUTH0_DOMAIN`, `VITE_AUTH0_CLIENT_ID`, `VITE_AUTH0_AUDIENCE`: Frontend Auth0 config

**AWS Configuration** (LocalStack in development):
- `AWS__UseLocalStack=true`: Use LocalStack instead of real AWS
- `AWS__LocalStackUrl=http://localhost:4566`
- `AWS__SNS__*TopicArn`: SNS topic ARNs
- `AWS__SQS__*QueueUrl`: SQS queue URLs

**Service Ports**:
- API Gateway: 5149
- Twitter Scanner: 5147
- Alert Service: 5148
- Token Metrics: Not yet assigned
- Frontend: 3000 (dev), proxies to API Gateway

### CORS Configuration
Configured in each service's `Program.cs` to allow:
- `http://localhost:3000`, `http://localhost:5173` (Vite dev servers)
- `https://localhost:3000`, `https://localhost:5173` (HTTPS variants)

## Key Dependencies

### Backend (.NET 8)
- **MediatR**: CQRS implementation for command/query separation within services
- **AWSSDK.SimpleNotificationService**: AWS SNS client for publishing events
- **AWSSDK.SQS**: AWS SQS client for consuming messages
- **ASP.NET Core 8.0**: Web API framework with SignalR support
- **Solnet.Rpc**: Solana blockchain interaction for token metadata
- **Microsoft.AspNetCore.Authentication.JwtBearer**: Auth0 JWT token validation
- **DotNetEnv**: Load `.env` files into environment variables
- **xUnit**: Testing framework (test projects not yet implemented)

### Frontend (React + TypeScript)
- **React 19.1.0**: UI framework with modern hooks and concurrent features
- **@auth0/auth0-react 2.6.0**: Auth0 authentication with React hooks
- **@microsoft/signalr 8.0.7**: Real-time communication with Alert Service hubs
- **react-router-dom 7.9.2**: Client-side routing
- **styled-components 6.1.19**: CSS-in-JS styling solution
- **kbar 0.1.0-beta.48**: Command palette UI component
- **Vite 7.0.0**: Build tool and development server with fast HMR
- **TypeScript 5.8.3**: Type-safe JavaScript with strict type checking

## Development Notes

### Clean Architecture Pattern
Each microservice follows Clean Architecture with clear layer separation:
```
Domain/           - Entities, enums, value objects (no dependencies)
Application/      - Business logic, MediatR handlers, interfaces (depends on Domain)
Infrastructure/   - External concerns: AWS, HTTP, SignalR (depends on Application)
API/              - Web API, Program.cs, Controllers (depends on all layers)
```

**Layer Registration**: Each layer has `DependencyInjection.cs` with `Add{Layer}()` extension method called in `Program.cs`

### Microservices Communication Architecture
- **Inter-service**: AWS SNS/SQS only - no direct HTTP calls between services
- **Client-to-service**: Via API Gateway (single entry point) or direct to Alert Service for SignalR
- **Event Contracts**: Shared in `CienceTerminal.Contracts` - versioned events with `AlertType` discriminator
- **Shared Infrastructure**: `CienceTerminal.AWS` provides reusable SNS/SQS clients and `SqsEventConsumer` base class

### AWS SNS/SQS Implementation Details
**Publishing** (Twitter Scanner):
1. Create event (e.g., `CaMentionAlertEvent`)
2. Serialize to JSON
3. Publish to SNS topic via `ISnsEventPublisher`
4. SNS fans out to subscribed SQS queues

**Consuming** (Alert Service):
1. `SqsEventConsumer<TEvent>` background service polls SQS queue
2. Deserializes JSON to event type
3. Publishes via MediatR to handler
4. Deletes message from queue on success

**Key Classes**:
- `SnsEventPublisher`: Publishes to SNS topics
- `SqsEventConsumer<TEvent>`: Generic background service for consuming SQS messages
- Event classes in `CienceTerminal.Contracts/Events/`

### Authentication & Authorization
- **Auth0 Integration**: JWT bearer tokens validated at API Gateway and SignalR hubs
- **Custom Claims**: `https://cienceterminal.com/tier` claim determines user tier (free/premium)
- **SignalR Auth**: Tokens passed via query string (`?access_token=`) for WebSocket connections
- **Tier-Based Groups**: Users added to SignalR groups based on tier for delayed alerts

### AI Integration (Twitter Scanner)
- **Groq API**: Fast LLM inference for tweet classification (crypto-related or not)
- **CA Detection**: Regex-based Solana address extraction from tweet text
- **Trend Analysis**: `CaTrackerService` tracks mention velocity and calculates trend state (new/accelerating/declining/dying)
- **Asynchronous Processing**: Background service processes tweets continuously with error handling

### Solana Integration
- **Jupiter Aggregator**: Token metadata, price, and market cap data
- **Helius RPC**: Solana blockchain RPC endpoint for token validation and on-chain data
- Configured via `ApiKeys__Helius` and `Endpoints__Helius` environment variables

### Testing Strategy (Planned)
- Test projects not yet implemented
- Planned structure: `services/{service}/tests/{Service}.UnitTests` and `{Service}.IntegrationTests`
- Integration tests should use LocalStack for AWS services
- Unit tests focus on Domain and Application layers (no infrastructure dependencies)

### Development Modes
**Hybrid Mode** (recommended for development):
- LocalStack in Docker for AWS SNS/SQS
- Services run natively with `dotnet run` for fast iteration
- Frontend runs with `npm run dev` for hot reload

**Full Docker Mode**:
- All services containerized
- Slower iteration but closer to production
- Useful for testing full stack integration

**Frontend-Only Mode**:
- Backend in Docker
- Frontend native for rapid UI development