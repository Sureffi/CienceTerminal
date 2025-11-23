import type { TwitterAlert } from '@/models/Alert';
import type { Token } from '@/types/token';

/**
 * Twitter Alert to Token Transformer
 *
 * Converts TwitterAlert data from the backend into Token format
 * for display in the TerminalPage.
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
 * Transform a TwitterAlert into a Token object
 */
export const transformTwitterAlertToToken = (alert: TwitterAlert): Token => {
    // Calculate current price from market cap (rough estimate)
    // Assuming 1B token supply for price calculation
    const coinMarketCap = 'coinMarketCap' in alert ? alert.coinMarketCap : undefined;
    const estimatedPrice = coinMarketCap ? coinMarketCap / 1_000_000_000 : 0.05;

    const token: Token = {
        id: alert.id,
        symbol: alert.coinSymbol,
        name: alert.coinName,
        blockchain: 'SOL', // Based on your Solana-focused architecture
        contractAddress: alert.coinMint,
        iconUrl: 'coinImageUrl' in alert ? alert.coinImageUrl : undefined, // Token icon from metadata service

        // Metrics - now available from coin metrics
        mentions24h: 'coinMentionCount24h' in alert ? (alert.coinMentionCount24h ?? 0) : 0,
        topMentioners: [
            {
                id: alert.authorUsername,
                username: alert.authorUsername,
                avatarUrl: alert.authorProfilePicture, // Tweet author's profile picture
            }
        ],

        // Price data
        priceHistory: generatePriceHistory(estimatedPrice),
        currentPrice: estimatedPrice,
        priceChange24h: undefined, // Not available in current data

        // Token stats from alert
        age: 'coinAge' in alert ? (alert.coinAge || 'Unknown') : 'Unknown',
        marketCap: coinMarketCap || 0,
        volume24h: 'coinVolume24h' in alert ? (alert.coinVolume24h ?? 0) : 0,
        liquidity: 'coinLiquidity' in alert ? (alert.coinLiquidity || 0) : 0,
        holdersCount: alert.coinHolderCount || 0,
        top10HoldersPercent: 'coinTop10Holders' in alert ? (alert.coinTop10Holders || 0) : 0,
        devHoldPercent: 'coinDevHolding' in alert ? (alert.coinDevHolding || 0) : 0,

        // Tweet context from alert
        tweet: {
            id: alert.tweetLink.split('/').pop() || alert.id,
            author: {
                username: alert.authorUsername,
                displayName: alert.authorName,
                verified: alert.authorIsBlueVerified || alert.authorIsGoldVerified,
                followers: alert.authorFollowers,
                avatarUrl: alert.authorProfilePicture, // Tweet author's profile picture
            },
            text: alert.tweetContent,
            createdAt: new Date(alert.timestamp),
            tweetUrl: alert.tweetLink,
        },

        createdAt: new Date(alert.timestamp),
    };

    return token;
};

/**
 * Transform multiple TwitterAlerts into Tokens
 */
export const transformTwitterAlertsToTokens = (alerts: TwitterAlert[]): Token[] => {
    return alerts.map(transformTwitterAlertToToken);
};
