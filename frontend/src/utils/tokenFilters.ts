import type { Token } from '@/types/token';
import type { TokenFilters } from '@/components/molecules/FilterPanel';

/**
 * Parse age string to hours
 * Converts formats like "3H", "2D", "1W" to hours
 */
const parseAgeToHours = (age: string): number => {
    const match = age.match(/^(\d+)([HDWM])$/i);
    if (!match) return 0;

    const value = parseInt(match[1], 10);
    const unit = match[2].toUpperCase();

    switch (unit) {
        case 'H':
            return value;
        case 'D':
            return value * 24;
        case 'W':
            return value * 24 * 7;
        case 'M':
            return value * 24 * 30; // Approximate month
        default:
            return 0;
    }
};

/**
 * Apply filters to a list of tokens
 * Returns filtered array based on provided filter criteria
 */
export const applyTokenFilters = (tokens: Token[], filters: TokenFilters): Token[] => {
    return tokens.filter((token) => {
        // Market Cap filters
        if (filters.marketCapMin !== undefined && token.marketCap < filters.marketCapMin) {
            return false;
        }
        if (filters.marketCapMax !== undefined && token.marketCap > filters.marketCapMax) {
            return false;
        }

        // Volume filters
        if (filters.volume24hMin !== undefined && token.volume24h < filters.volume24hMin) {
            return false;
        }
        if (filters.volume24hMax !== undefined && token.volume24h > filters.volume24hMax) {
            return false;
        }

        // Liquidity filters
        if (filters.liquidityMin !== undefined && token.liquidity < filters.liquidityMin) {
            return false;
        }
        if (filters.liquidityMax !== undefined && token.liquidity > filters.liquidityMax) {
            return false;
        }

        // Holders count filters
        if (filters.holdersCountMin !== undefined && token.holdersCount < filters.holdersCountMin) {
            return false;
        }
        if (filters.holdersCountMax !== undefined && token.holdersCount > filters.holdersCountMax) {
            return false;
        }

        // Top 10 holders percentage filter
        if (filters.top10HoldersPercentMax !== undefined && token.top10HoldersPercent > filters.top10HoldersPercentMax) {
            return false;
        }

        // Dev hold percentage filter
        if (filters.devHoldPercentMax !== undefined && token.devHoldPercent > filters.devHoldPercentMax) {
            return false;
        }

        // Mentions filter
        if (filters.mentions24hMin !== undefined && token.mentions24h < filters.mentions24hMin) {
            return false;
        }

        // Trend score filter
        if (filters.trendScoreMin !== undefined) {
            const trendScore = token.trendScore ?? 0;
            if (trendScore < filters.trendScoreMin) {
                return false;
            }
        }

        // Age filter (convert age string to hours)
        if (filters.ageHoursMin !== undefined || filters.ageHoursMax !== undefined) {
            const ageHours = parseAgeToHours(token.age);
            if (filters.ageHoursMin !== undefined && ageHours < filters.ageHoursMin) {
                return false;
            }
            if (filters.ageHoursMax !== undefined && ageHours > filters.ageHoursMax) {
                return false;
            }
        }

        return true;
    });
};

/**
 * Get count of active filters
 */
export const getActiveFilterCount = (filters: TokenFilters): number => {
    return Object.values(filters).filter(v => v !== undefined).length;
};
