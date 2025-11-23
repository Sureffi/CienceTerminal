# Token Metrics Service - Architecture Overview

## Service Responsibility

The Token Metrics Service is responsible for **real-time token metadata management** in the CienceTerminal platform. It decouples token data fetching from tweet processing, enabling:

- **Focused token analytics** without coupling to Twitter Scanner
- **Tiered caching strategy** (Hot/Warm/Cold) based on trending status
- **Centralized API integration** with external data providers
- **Real-time updates** for top 25 trending tokens only

## Data Flow Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    ALERT SERVICE                             │
├─────────────────────────────────────────────────────────────┤
│  • Calculates top 25 trending tokens                         │
│  • Publishes TokenTrendingListUpdatedEvent                   │
│    Topic: cienceterminal-token-trending-list                 │
└─────────────────────────────────────────────────────────────┘
                             │
                             ▼ SNS → SQS
┌─────────────────────────────────────────────────────────────┐
│              TOKEN METRICS SERVICE (This)                    │
├─────────────────────────────────────────────────────────────┤
│  1. Consumes TokenTrendingListUpdatedEvent via SQS          │
│  2. Updates cache tier: Promote top 25 to "Hot"            │
│  3. Background job runs every 1-2 minutes:                  │
│     └─ Fetches Jupiter API for all "Hot" tokens            │
│  4. Publishes TokenMetricsUpdatedEvent to SNS               │
│     Topic: cienceterminal-token-metrics-updated              │
└─────────────────────────────────────────────────────────────┘
                             │
                             ▼ SNS → SQS
┌─────────────────────────────────────────────────────────────┐
│                    ALERT SERVICE                             │
├─────────────────────────────────────────────────────────────┤
│  • Consumes TokenMetricsUpdatedEvent                         │
│  • Enriches CaMentionAlert with latest metrics              │
│  • Broadcasts updated alert via SignalR to frontend         │
└─────────────────────────────────────────────────────────────┘
                             │
                             ▼ SignalR
┌─────────────────────────────────────────────────────────────┐
│                       FRONTEND                               │
├─────────────────────────────────────────────────────────────┤
│  • Receives real-time token metrics updates                 │
│  • Displays: Symbol, Market Cap, Volume, Holders, etc.     │
└─────────────────────────────────────────────────────────────┘
```

## Cache Tiers

### Hot Tier (Top 25 Trending)
- **Update frequency:** Every 1-2 minutes
- **Trigger:** Token enters top 25 trending list
- **Purpose:** Real-time metrics for actively trending tokens
- **Data:** Full Jupiter metadata

### Warm Tier (Recently Searched)
- **Update frequency:** Every 15 minutes
- **Trigger:** User searches for token, or token drops from top 25
- **Purpose:** Quick response for recent searches
- **Data:** Cached Jupiter metadata

### Cold Tier (Historical)
- **Update frequency:** On-demand only
- **Trigger:** Initial token discovery, or aged out from Warm
- **Purpose:** Long-term storage, minimal API calls
- **Data:** Stale metadata, refreshed on explicit request

## Event Contracts

### Incoming: TokenTrendingListUpdatedEvent
```csharp
{
    UpdatedAt: DateTime,
    Top25Tokens: [
        {
            ContractAddress: string,
            TrendingScore: decimal,
            Rank: int  // 1-25
        }
    ]
}
```

### Outgoing: TokenMetricsUpdatedEvent
```csharp
{
    ContractAddress: string,
    UpdatedAt: DateTime,

    // Basic
    Symbol: string,
    Name: string?,
    IconUrl: string?,

    // Financial
    MarketCap: decimal?,
    Liquidity: decimal?,
    UsdPrice: decimal?,

    // Holders
    HolderCount: int?,
    Top10HoldersPercent: decimal?,
    DevHoldingPercent: decimal?,

    // Security
    IsMintDisabled: bool?,
    IsFreezeDisabled: bool?,

    // Metadata
    IsVerified: bool?,
    Launchpad: string?,
    FirstPoolCreatedAt: DateTime?,

    // Future: DexScreener
    Volume24h: decimal?,
    PriceChange24h: decimal?
}
```

## External API Integration

### Jupiter Aggregator Lite API (v1)
- **Endpoint:** `https://lite-api.jup.ag/ultra/v1/search?query={mint}`
- **Returns:** Token metadata, financial data, holder stats
- **Rate limits:** Unknown, implement exponential backoff
- **Caching:** Required to minimize API calls

### DexScreener API (Future)
- **Purpose:** 24h volume, price history for sparklines
- **Rate limits:** 300 req/min (free tier)
- **Integration:** Parallel to Jupiter in background job

## Performance Considerations

### API Efficiency
- **Top 25 only:** Limits refresh to 25 tokens every 1-2 min = ~750 requests/hour
- **Batch processing:** All Hot tier tokens refreshed in single job iteration
- **Error handling:** Failed requests don't block other tokens

### Scalability
- **In-memory cache:** Fast lookups for Warm/Cold tiers
- **Database persistence:** (Future) EF Core for long-term storage
- **Horizontal scaling:** Stateless design allows multiple instances

### Cost Optimization
- **Tiered updates:** Only frequent updates for trending tokens
- **Automatic demotion:** Tokens drop to Warm tier when leaving top 25
- **On-demand refresh:** Cold tier only updated when explicitly requested

## Technology Stack

- **.NET 9.0** - Runtime
- **ASP.NET Core** - Web API framework
- **MediatR** - CQRS pattern (commands/queries)
- **AWS SNS/SQS** - Event-driven messaging
- **HttpClient** - External API calls (Jupiter, DexScreener)
- **EF Core** - (Future) Database persistence

## Future Enhancements

1. **Database persistence** - EF Core with PostgreSQL/SQL Server
2. **DexScreener integration** - Volume and price history
3. **Redis caching** - Distributed cache for horizontal scaling
4. **GraphQL API** - Flexible querying for frontend
5. **Metrics dashboard** - Prometheus + Grafana monitoring
6. **Rate limit handling** - Exponential backoff, circuit breaker
7. **Webhook support** - Push notifications for metric changes
