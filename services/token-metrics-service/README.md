# Token Metrics Service

The **Token Metrics Service** is a dedicated microservice in the CienceTerminal platform responsible for fetching, caching, and refreshing cryptocurrency token metadata from external APIs.

## Purpose

This service decouples token metadata management from tweet processing, providing:
- **Real-time token metrics** for the top 25 trending tokens
- **Centralized API integration** with Jupiter Aggregator and (future) DexScreener
- **Intelligent caching** with tiered update frequencies (Hot/Warm/Cold)
- **Event-driven architecture** via AWS SNS/SQS

## Architecture

### Clean Architecture Layers

```
TokenMetrics.API          → REST endpoints, Swagger docs
TokenMetrics.Application  → CQRS (MediatR), business logic
TokenMetrics.Infrastructure → External APIs (Jupiter), AWS messaging
TokenMetrics.Domain       → Entities, interfaces, no dependencies
```

### Dependencies

- **Shared Libraries:**
  - `CienceTerminal.Contracts` - Shared event contracts
  - `CienceTerminal.AWS` - AWS SNS/SQS clients

- **External APIs:**
  - Jupiter Aggregator Lite API (v1)
  - DexScreener API (future)

## Data Flow

```
1. Alert Service publishes TokenTrendingListUpdatedEvent → SNS
2. Token Metrics Service receives event → SQS
3. Background job refreshes top 25 tokens (every 1-2 min)
4. Publishes TokenMetricsUpdatedEvent → SNS
5. Alert Service receives and updates CaMentionAlert with metrics
```

## Event Contracts

### Consumes

- **TokenTrendingListUpdatedEvent** (`cienceterminal-token-trending-list`)
  - List of top 25 trending tokens with contract addresses
  - Triggers promotion to "Hot" cache tier

### Produces

- **TokenMetricsUpdatedEvent** (`cienceterminal-token-metrics-updated`)
  - Complete token metadata (symbol, market cap, holders, liquidity, etc.)
  - Consumed by Alert Service to enrich CA mention alerts

## Domain Entities

### TokenMetadata

Complete token information combining data from multiple sources:
- **Basic Info:** Symbol, name, icon, decimals
- **Financial:** Market cap, liquidity, USD price, FDV
- **Holders:** Count, top 10%, dev holding %
- **Security:** Mint/freeze disabled status
- **Social:** Twitter, Telegram, website links
- **Cache Tier:** Hot (1-2 min updates), Warm (15 min), Cold (on-demand)

### JupiterTokenData

Response structure from Jupiter Aggregator API with JSON property mappings.

## API Endpoints (TODO)

```
GET /api/tokens/search?q={query}     # Search by symbol or CA
GET /api/tokens/{ca}/metrics          # Get specific token metrics
GET /api/tokens/metadata              # Get basic info for all tracked tokens
```

## Configuration

### appsettings.json

```json
{
  "AWS": {
    "ServiceURL": "http://localhost:4566",  # LocalStack
    "Profile": "localstack",
    "Region": "us-east-1"
  }
}
```

### Environment Variables (Production)

- `AWS__ServiceURL` - AWS endpoint (empty for real AWS)
- `AWS__Region` - AWS region

## Running the Service

### Development

```bash
cd services/token-metrics-service/src/TokenMetrics.API
dotnet run
```

Service runs on `http://localhost:5000` (or configured port)
Swagger UI: `http://localhost:5000/swagger`

### With LocalStack (AWS SNS/SQS)

```bash
docker-compose up localstack
./scripts/setup-aws-resources.sh
dotnet run --project services/token-metrics-service/src/TokenMetrics.API
```

## Implementation Status

### ✅ Completed
- [x] Project structure (Clean Architecture)
- [x] Domain entities (TokenMetadata, JupiterTokenData)
- [x] Shared event contracts (TokenTrendingListUpdatedEvent, TokenMetricsUpdatedEvent)
- [x] Jupiter API client interface and implementation
- [x] Dependency injection setup
- [x] Basic API with Program.cs configuration

### 🚧 TODO
- [ ] Token metrics repository (in-memory + database)
- [ ] Background job for refreshing top 25 tokens
- [ ] SQS consumer for TokenTrendingListUpdatedEvent
- [ ] SNS publisher for TokenMetricsUpdatedEvent
- [ ] API controllers for token search
- [ ] Cache tier management logic
- [ ] DexScreener API integration (volume, price history)
- [ ] Database persistence (EF Core)
- [ ] Unit and integration tests

## Next Steps

1. **Implement repository pattern** for TokenMetadata storage
2. **Create background job** (IHostedService) to refresh token metrics
3. **Add SNS/SQS messaging** handlers for event-driven communication
4. **Build REST API** for token search functionality
5. **Integrate with Alert Service** to test end-to-end data flow

## Contributing

Follow the existing Clean Architecture patterns in Twitter Scanner and Alert Service.
