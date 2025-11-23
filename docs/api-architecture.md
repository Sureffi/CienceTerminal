# API Architecture Documentation

## Overview

CienceTerminal uses a **microservices architecture** with an API Gateway pattern for centralized routing, authentication, and request forwarding. The system implements versioned REST APIs and dedicated SignalR WebSocket hubs for real-time communication.

## Architecture Components

### 1. API Gateway (Port 5149)
- **Technology**: YARP (Yet Another Reverse Proxy) on .NET 8
- **Responsibilities**:
  - JWT authentication validation (Auth0)
  - Route management and versioning
  - Request forwarding to downstream services
  - Internal endpoint blocking
  - User context propagation via headers

### 2. Microservices
- **Alert Service** (Port 5148): Alert processing and real-time distribution
- **Twitter Scanner Service** (Port 5147): Twitter stream processing and CA detection
- **User Management Service**: User profile and tier management (planned)

---

## API Routing Patterns

### Public API Endpoints
All public APIs follow this pattern:
```
/api/v1/{resource}/{action}
```

**Current Version**: v1
**Versioning Strategy**: Gateway-controlled (services are version-agnostic internally)

### SignalR Hubs
WebSocket hubs use dedicated paths:
```
/alerts/hub/{alert-type}
```

### Internal Endpoints
Internal service-to-service endpoints (blocked at gateway):
```
/internal/{service}/{action}
```

---

## Gateway Configuration

### Route Handling

#### 1. Internal Route Blocking
```json
"block-internal-routes": {
  "ClusterId": "blocked",
  "Match": { "Path": "/internal/{**catch-all}" },
  "Order": 1
}
```
- **Purpose**: Prevent external access to internal APIs
- **Behavior**: Routes to non-existent destination (http://localhost:1)
- **Priority**: Order 1 (highest)

#### 2. REST API Routes

**Alerts Endpoints**
```json
"v1-alerts-route": {
  "ClusterId": "alert-service",
  "Match": { "Path": "/api/v1/alerts/{**catch-all}" },
  "Transforms": [{ "PathPattern": "/api/alerts/{**catch-all}" }],
  "AuthorizationPolicy": "Default"
}
```
- **External Path**: `/api/v1/alerts/*`
- **Internal Path**: `/api/alerts/*` (version stripped)
- **Authentication**: Required (Auth0 JWT)

**CA Mention Details**
```json
"v1-alerts-ca-details-route": {
  "ClusterId": "twitter-scanner",
  "Match": { "Path": "/api/v1/alerts/ca-mentions/{address}" },
  "Transforms": [{ "PathPattern": "/api/ca-mentions/{address}" }],
  "AuthorizationPolicy": "Default",
  "Order": 1
}
```
- **External Path**: `/api/v1/alerts/ca-mentions/{address}`
- **Internal Path**: `/api/ca-mentions/{address}`
- **Service**: Twitter Scanner
- **Priority**: Order 1 (evaluated before general alerts route)

**User Endpoints**
```json
"v1-users-route": {
  "ClusterId": "user-management",
  "Match": { "Path": "/api/v1/users/{**catch-all}" },
  "Transforms": [{ "PathPattern": "/api/users/{**catch-all}" }],
  "AuthorizationPolicy": "Default"
}
```
- **External Path**: `/api/v1/users/*`
- **Internal Path**: `/api/users/*`
- **Service**: User Management

#### 3. SignalR Hub Routes

**Twitter Alerts Hub**
```json
"twitter-alert-hub-route": {
  "ClusterId": "alert-service",
  "Match": { "Path": "/alerts/hub/twitter/{**catch-all}" },
  "Transforms": [{ "PathPattern": "/twitter-alert-hub/{**catch-all}" }]
}
```
- **External Path**: `/alerts/hub/twitter`
- **Internal Path**: `/twitter-alert-hub`
- **Authentication**: Optional (tier-based via JWT claims)

**CA Mention Alerts Hub**
```json
"ca-mention-alert-hub-route": {
  "ClusterId": "alert-service",
  "Match": { "Path": "/alerts/hub/ca-mentions/{**catch-all}" },
  "Transforms": [{ "PathPattern": "/ca-mention-alert-hub/{**catch-all}" }]
}
```
- **External Path**: `/alerts/hub/ca-mentions`
- **Internal Path**: `/ca-mention-alert-hub`
- **Authentication**: Optional (tier-based via JWT claims)

---

## Authentication & Authorization

### Auth0 JWT Configuration
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://{auth0Domain}/";
        options.Audience = auth0Audience;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.NameIdentifier
        };
    });
```

**Environment Variables**:
- `AUTH0_DOMAIN`: Auth0 tenant domain
- `AUTH0_AUDIENCE`: API identifier (e.g., `https://cienceterminal-api`)

### User Context Forwarding
Gateway extracts JWT claims and forwards them as headers to downstream services:

```csharp
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var auth0Sub = context.User.FindFirst("sub")?.Value;
        var email = context.User.FindFirst("email")?.Value;
        var emailVerified = context.User.FindFirst("email_verified")?.Value;

        context.Request.Headers["X-User-Auth0-Sub"] = auth0Sub;
        context.Request.Headers["X-User-Email"] = email;
        context.Request.Headers["X-User-Email-Verified"] = emailVerified;
    }
    await next();
});
```

**Forwarded Headers**:
- `X-User-Auth0-Sub`: Auth0 user ID (sub claim)
- `X-User-Email`: User email address
- `X-User-Email-Verified`: Email verification status

---

## Service Endpoints

### Alert Service

#### REST Endpoints

**Base Path (Internal)**: `/api/alerts`

##### Get Twitter Alerts
```http
GET /api/alerts/twitter
```
**Response**: List of active Twitter alerts (TwitterAlert[])

##### Get CA Mention Alerts
```http
GET /api/alerts/ca-mentions
```
**Response**: List of active CA mention alerts (CaMentionAlert[])

**Controller Location**: `AlertService.API/Controllers/Public/AlertsController.cs`

#### SignalR Hubs

##### Twitter Alert Hub
**Path (Internal)**: `/twitter-alert-hub`
**External Path**: `/alerts/hub/twitter`

**Client Methods**:
- `GetActiveTwitterAlerts()`: Fetch active Twitter alerts
- `RemoveAlert(string alertId)`: Remove specific alert

**Server Events**:
- `AlertAdded`: New Twitter alert (delayed by tier)
- `AlertRemoved`: Alert removed

**Tier-based Groups**:
- `twitter-alerts-premium`: Instant delivery
- `twitter-alerts-pro`: 1-second delay
- `twitter-alerts-free`: 5-second delay

##### CA Mention Alert Hub
**Path (Internal)**: `/ca-mention-alert-hub`
**External Path**: `/alerts/hub/ca-mentions`

**Client Methods**:
- `GetActiveCaMentionAlerts()`: Fetch active CA mention alerts
- `RemoveAlert(string alertId)`: Remove specific alert

**Server Events**:
- `AlertAdded`: New CA mention alert (delayed by tier)
- `AlertRemoved`: Alert removed

**Tier-based Groups**:
- `ca-mention-alerts-premium`: Instant delivery
- `ca-mention-alerts-pro`: 1-second delay
- `ca-mention-alerts-free`: 5-second delay

**Hub Location**: `AlertService.Infrastructure/Hubs/`

### Twitter Scanner Service

#### REST Endpoints

**Base Path (Internal)**: `/api/ca-mentions`

##### Get CA Mention Details
```http
GET /api/ca-mentions/{coinAddress}?hours=24
```

**Parameters**:
- `coinAddress` (path): Solana contract address
- `hours` (query, optional): Lookback period (1-168, default: 24)

**Response**: Detailed CA mention metrics and tweet history

**Controller Location**: `TwitterScanner.API/Controllers/Public/CaMentionController.cs`

---

## Tier-Based Alert Delivery

### SignalR Hub Connection Flow

1. **Client connects** to hub (Twitter or CA Mention)
2. **Hub reads JWT claims**:
   ```csharp
   var tier = Context.User?.FindFirst("https://cienceterminal.com/tier")?.Value ?? "free";
   var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;
   ```
3. **User added to tier-specific group**:
   ```csharp
   await Groups.AddToGroupAsync(Context.ConnectionId, $"twitter-alerts-{tier}");
   ```
4. **Alerts distributed with delays**:
   - Premium: Immediate (`SendAsync`)
   - Pro: 1-second delay (`Task.Delay(1000)`)
   - Free: 5-second delay (`Task.Delay(5000)`)

### Custom JWT Claims
**Tier Claim**: `https://cienceterminal.com/tier`
**Values**: `free`, `pro`, `premium`

---

## CORS Configuration

### Gateway CORS
```csharp
policy
    .WithOrigins("http://localhost:3000", "http://localhost:5173")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials();
```

### Service CORS
Each service independently configures CORS for direct access (development):
- Allowed origins: `localhost:3000`, `localhost:5173`
- Credentials: Enabled (for SignalR)

---

## Controller Organization

### Directory Structure
```
Controllers/
├── Public/          # Externally accessible via gateway
│   └── AlertsController.cs
└── Internal/        # Service-to-service only (blocked at gateway)
    └── ...
```

### Naming Conventions
- **Public Controllers**: Explicit routes (`[Route("api/alerts")]`)
- **Internal Controllers**: Explicit routes (`[Route("internal/...")]`)
- **No `[controller]` token**: Routes are explicit for clarity

---

## Service Discovery & Clustering

### Current Setup (Docker Compose)
```json
"Clusters": {
  "alert-service": {
    "Destinations": {
      "destination1": { "Address": "http://alert-service:8080" }
    }
  },
  "twitter-scanner": {
    "Destinations": {
      "destination1": { "Address": "http://twitter-scanner:8080" }
    }
  }
}
```

### Future: Consul Integration
- Dynamic service registration
- Health checks
- Load balancing across multiple instances

---

## API Versioning Strategy

### Gateway-Controlled Versioning
1. **Clients use versioned paths**: `/api/v1/*`
2. **Gateway strips version**: Transforms to `/api/*`
3. **Services remain version-agnostic**: No version in internal routes
4. **Version changes**: Only update gateway transforms

**Benefits**:
- Centralized version management
- Services don't need version-aware routing
- Easy to add v2 routes without service changes

### Future Versions
To add v2:
```json
"v2-alerts-route": {
  "ClusterId": "alert-service-v2",
  "Match": { "Path": "/api/v2/alerts/{**catch-all}" },
  "Transforms": [{ "PathPattern": "/api/alerts/{**catch-all}" }]
}
```

---

## Security Architecture

### Defense in Depth
1. **Gateway Layer**:
   - JWT validation (Auth0)
   - Internal route blocking
   - CORS enforcement

2. **Service Layer** (Future):
   - Header-based user context trust
   - Optional JWT validation for critical operations
   - Authorization policies based on user tier

3. **Network Layer**:
   - Services not directly exposed to internet
   - Gateway as single entry point

---

## Development & Deployment

### Local Development
- Gateway: `http://localhost:5149`
- Alert Service: `http://localhost:5148` (direct access)
- Twitter Scanner: `http://localhost:5147` (direct access)
- Frontend: `http://localhost:3000` or `http://localhost:5173`

### Docker Deployment
All services exposed internally on port 8080:
- Gateway: External 5149 → Internal 8080
- Services communicate via Docker network
- Service names used for DNS resolution

### Environment Variables
**Gateway**:
- `AUTH0_DOMAIN`
- `AUTH0_AUDIENCE`
- `API_GATEWAY_PORT` (default: 5149)

**Services**:
- Service-specific port variables (e.g., `ALERT_SERVICE_PORT`)

---

## Frontend Integration

### API Configuration (`frontend/src/config/api.ts`)

```typescript
export const API_ENDPOINTS = {
  alerts: {
    twitter: `${API_BASE_URL}/api/v1/alerts/twitter`,
    caMentions: `${API_BASE_URL}/api/v1/alerts/ca-mentions`,
    remove: (alertId: string) => `${API_BASE_URL}/api/v1/alerts/${alertId}`,
  },
  caMentions: {
    details: (coinAddress: string, hours: number) =>
      `${API_BASE_URL}/api/v1/alerts/ca-mentions/${coinAddress}?hours=${hours}`,
  },
  users: {
    me: `${API_BASE_URL}/api/v1/users/me`,
  }
} as const;

export const SIGNALR_HUBS = {
  twitter: `${apiConfig.signalrHubUrl}/alerts/hub/twitter`,
  caMentions: `${apiConfig.signalrHubUrl}/alerts/hub/ca-mentions`,
} as const;
```

### SignalR Connection Example
```typescript
const connection = new HubConnectionBuilder()
  .withUrl(SIGNALR_HUBS.twitter, {
    accessTokenFactory: async () => await getAccessTokenSilently()
  })
  .withAutomaticReconnect()
  .configureLogging(LogLevel.Information)
  .build();

connection.on("AlertAdded", (alert: TwitterAlert) => {
  // Handle new alert
});
```

---

## Error Handling

### Gateway Errors
- **401 Unauthorized**: Invalid/missing JWT
- **403 Forbidden**: Valid JWT but insufficient permissions
- **404 Not Found**: Route to blocked internal endpoint

### Service Errors
- **400 Bad Request**: Invalid parameters
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Service errors

### SignalR Errors
- **Connection Failure**: Auto-reconnect enabled
- **Method Not Found**: Client/server version mismatch

---

## Monitoring & Health Checks

### Gateway Health
```http
GET /health
```
Returns: `Healthy` or `Unhealthy`

### Service Health
Each service exposes `/health` endpoint (internal access only)

### Logging
- Console logging enabled for all services
- Log levels configurable via environment

---

## Future Enhancements

1. **Rate Limiting**: Per-tier API rate limits
2. **API Analytics**: Request tracking and metrics
3. **GraphQL Gateway**: Alternative to REST for complex queries
4. **WebSocket Authentication**: JWT validation for SignalR connections
5. **Service Mesh**: Istio/Linkerd for advanced traffic management
6. **API Documentation**: Swagger UI exposed via gateway
