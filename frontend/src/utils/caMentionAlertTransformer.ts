import type { CaMentionAlert } from '@/models/Alert';
import type { Token } from '@/types/token';

/**
 * CA Mention Alert to Token Transformer
 *
 * Converts CaMentionAlert data (aggregated mention statistics) into Token format
 * for display in the ScreenerPage.
 */

// Helper to generate price history from base price
const generatePriceHistory = (basePrice: number = 0.05, volatility: number = 0.2): number[] => {
    const points = 20;
    const history: number[] = [];
    let currentPrice = basePrice;

    for (let i = 0; i < points; i++) {
        const change = (Math.random() - 0.5) * volatility * currentPrice;
        currentPrice += change;
        history.push(Math.max(0, currentPrice));
    }

    return history;
};

/**
 * Transform a CaMentionAlert into a Token object
 */
export const transformCaMentionAlertToToken = (alert: CaMentionAlert): Token => {
    // Estimate price from market cap if available (rough estimate)
    // Assuming 1B token supply for calculation
    const estimatedPrice = 0.05; // Placeholder - would need market data

    // Calculate age from firstPoolCreatedAt or coinFirstPoolCreatedAt
    const poolCreatedAt = alert.firstPoolCreatedAt || alert.coinFirstPoolCreatedAt;
    const age = poolCreatedAt ? calculateAge(poolCreatedAt) : alert.coinAddress;

    // Transform top mentioner image URLs into TokenMentioner objects
    const topMentioners = alert.topMentionerImageUrls?.map((url, index) => ({
        id: `${alert.id}-mentioner-${index}`,
        avatarUrl: url,
        username: undefined, // Username not available in aggregated data
    })) || [];

    const token: Token = {
        id: alert.id,
        symbol: alert.coinSymbol,
        name: alert.coinSymbol, // CA mentions don't have full name
        blockchain: 'SOL', // Based on Solana-focused architecture
        contractAddress: alert.coinAddress,
        iconUrl: alert.coinImageUrl, // Token icon from metadata service

        // Mention metrics - this is the key data from CA mention alerts!
        mentions24h: alert.mentionCount24Hour,
        trendScore: alert.trendScore,
        topMentioners,

        // Price data (placeholder - needs integration with Jupiter API)
        priceHistory: generatePriceHistory(estimatedPrice),
        currentPrice: estimatedPrice,
        priceChange24h: alert.priceChange24H ?? undefined,

        // Token stats - now properly mapped from alert data!
        age,
        marketCap: alert.marketCap ?? 0,
        volume24h: alert.volume24h ?? 0,
        liquidity: alert.liquidity ?? 0,
        holdersCount: alert.holderCount ?? 0,
        top10HoldersPercent: alert.topHoldersPercentage ?? 0,
        devHoldPercent: 0, // Not available in CA mention alerts

        // No tweet context for screener view (shows aggregated data)
        tweet: undefined,

        createdAt: new Date(alert.timestamp),
    };

    return token;
};

/**
 * Calculate token age from pool creation timestamp
 */
const calculateAge = (timestamp: string): string => {
    const now = new Date();
    const created = new Date(timestamp);
    const diffMs = now.getTime() - created.getTime();
    const diffMinutes = Math.floor(diffMs / (1000 * 60));
    const diffHours = Math.floor(diffMinutes / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffDays > 0) return `${diffDays}D`;
    if (diffHours > 0) return `${diffHours}H`;
    return `${diffMinutes}M`;
};

/**
 * Transform multiple CaMentionAlerts into Tokens
 */
export const transformCaMentionAlertsToTokens = (alerts: CaMentionAlert[]): Token[] => {
    return alerts.map(transformCaMentionAlertToToken);
};
