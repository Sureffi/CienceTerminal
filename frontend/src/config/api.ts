// API Configuration for both development and production
const config = {
  development: {
    // In development, use the API Gateway running locally
    apiBaseUrl: import.meta.env.VITE_API_GATEWAY_URL || 'http://localhost:5149',
    signalrHubUrl: import.meta.env.VITE_API_GATEWAY_URL || 'http://localhost:5149',
  },
  production: {
    // In production, use VITE_API_GATEWAY_URL if set, otherwise use relative paths
    apiBaseUrl: import.meta.env.VITE_API_GATEWAY_URL || '',
    signalrHubUrl: import.meta.env.VITE_API_GATEWAY_URL || '',
  }
};

const environment = import.meta.env.MODE || 'development';
const apiConfig = config[environment as keyof typeof config];

export const API_BASE_URL = apiConfig.apiBaseUrl;

// SignalR Hub URLs (new pattern: /alerts/hub/{type})
export const SIGNALR_HUBS = {
  twitter: `${apiConfig.signalrHubUrl}/alerts/hub/twitter`,
  caMentions: `${apiConfig.signalrHubUrl}/alerts/hub/ca-mentions`,
} as const;

// Legacy hub URL (deprecated - use SIGNALR_HUBS instead)
export const SIGNALR_HUB_URL = SIGNALR_HUBS.twitter;

// API endpoints (v1)
export const API_ENDPOINTS = {
  alerts: {
    twitter: `${API_BASE_URL}/api/v1/alerts/twitter`,
    caMentions: `${API_BASE_URL}/api/v1/alerts/ca-mentions`,
    remove: (alertId: string) => `${API_BASE_URL}/api/v1/alerts/${alertId}`,
  },
  twitter: {
    base: `${API_BASE_URL}/api/v1/twitter`,
  },
  caMentions: {
    details: (coinAddress: string, hours: number) => `${API_BASE_URL}/api/v1/alerts/ca-mentions/${coinAddress}?hours=${hours}`,
  },
  mentions: {
    byCoin: (coinAddress: string, hours: number = 24) => `${API_BASE_URL}/api/v1/mentions/${coinAddress}?hours=${hours}`,
  },
  users: {
    me: `${API_BASE_URL}/api/v1/users/me`,
  }
} as const;