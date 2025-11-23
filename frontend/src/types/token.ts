/**
 * Token Data Types
 *
 * Represents cryptocurrency token information displayed in the screener table
 */

export interface TokenMentioner {
    id: string;
    avatarUrl?: string;
    username?: string;
}

export interface TweetAuthor {
    username: string;
    displayName?: string;
    avatarUrl?: string;
    verified?: boolean;
    followers: number;
}

export interface TweetData {
    id: string;
    author: TweetAuthor;
    text: string;
    createdAt: Date;
    tweetUrl?: string;
}

export interface Token {
    id: string;
    symbol: string;              // e.g., "KITKAT"
    name?: string;               // Full name if different from symbol
    blockchain: 'SOL' | 'ETH' | 'BASE' | 'ARB' | 'MATIC'; // Blockchain type
    iconUrl?: string;            // Token icon/logo

    // Metrics
    mentions24h: number;         // Number of mentions in last 24h
    trendScore?: number;         // EMA-based trend score for CA mentions
    topMentioners?: TokenMentioner[]; // Top users mentioning this token

    // Price/Chart data
    priceHistory: number[];      // Array of prices for sparkline chart
    currentPrice?: number;
    priceChange24h?: number;     // Percentage change

    // Token stats
    age: string;                 // e.g., "3H", "2D", "1W"
    marketCap: number;           // Market cap in USD
    volume24h: number;           // 24h volume in USD
    liquidity: number;           // Liquidity pool size in USD
    holdersCount: number;        // Number of token holders
    top10HoldersPercent: number; // % held by top 10 holders
    devHoldPercent: number;      // % held by dev wallet

    // Metadata
    contractAddress?: string;
    createdAt?: Date;

    // Tweet context (optional - only present when token is from a tweet)
    tweet?: TweetData;
}

export type TokenSortField =
    | 'mentions24h'
    | 'marketCap'
    | 'volume24h'
    | 'age'
    | 'liquidity'
    | 'holdersCount';

export type TokenFilterTab = 'TRENDING' | 'TOP' | 'NEW';
