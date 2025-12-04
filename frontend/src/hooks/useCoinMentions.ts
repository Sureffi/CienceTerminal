import { useState, useEffect } from 'react';
// import { useAuth0 } from '@auth0/auth0-react';
import { API_ENDPOINTS } from '@/config/api';

export interface CaMentionRecord {
    id: string;
    coinMintAddress: string;
    tweetId: string;
    authorId: string;
    username: string;
    profilePicture: string;
    tweetUrl: string;
    tweetContent: string | null;
    followers: number;
    isVerified: boolean;
    timestamp: string;
    isOriginalPost: boolean;
    isReply: boolean;
    isQuote: boolean;
    isRetweet: boolean;
}

// Mock mention data for demo mode
const mockMentions: Record<string, CaMentionRecord[]> = {
    'ABC123xyz456': [
        {
            id: 'mention-1',
            coinMintAddress: 'ABC123xyz456',
            tweetId: '1234567890',
            authorId: 'author1',
            username: 'cryptowhale',
            profilePicture: 'https://i.pravatar.cc/150?img=15',
            tweetUrl: 'https://twitter.com/cryptowhale/status/1234567890',
            tweetContent: 'Just discovered $MOON - the fundamentals look really strong! ABC123xyz456',
            followers: 125000,
            isVerified: true,
            timestamp: new Date(Date.now() - 5 * 60 * 1000).toISOString(),
            isOriginalPost: true,
            isReply: false,
            isQuote: false,
            isRetweet: false,
        },
        {
            id: 'mention-2',
            coinMintAddress: 'ABC123xyz456',
            tweetId: '1234567891',
            authorId: 'author2',
            username: 'tokentrader',
            profilePicture: 'https://i.pravatar.cc/150?img=22',
            tweetUrl: 'https://twitter.com/tokentrader/status/1234567891',
            tweetContent: 'Amazing potential here! $MOON looking bullish 🚀',
            followers: 45000,
            isVerified: false,
            timestamp: new Date(Date.now() - 12 * 60 * 1000).toISOString(),
            isOriginalPost: false,
            isReply: true,
            isQuote: false,
            isRetweet: false,
        },
        {
            id: 'mention-3',
            coinMintAddress: 'ABC123xyz456',
            tweetId: '1234567892',
            authorId: 'author3',
            username: 'defi_hunter',
            profilePicture: 'https://i.pravatar.cc/150?img=33',
            tweetUrl: 'https://twitter.com/defi_hunter/status/1234567892',
            tweetContent: 'This one is going to explode. Mark my words. ABC123xyz456',
            followers: 89000,
            isVerified: true,
            timestamp: new Date(Date.now() - 18 * 60 * 1000).toISOString(),
            isOriginalPost: false,
            isReply: false,
            isQuote: true,
            isRetweet: false,
        },
        {
            id: 'mention-4',
            coinMintAddress: 'ABC123xyz456',
            tweetId: '1234567893',
            authorId: 'author4',
            username: 'moon_tracker',
            profilePicture: 'https://i.pravatar.cc/150?img=44',
            tweetUrl: 'https://twitter.com/moon_tracker/status/1234567893',
            tweetContent: 'RT: Just discovered $MOON - the fundamentals look really strong!',
            followers: 34000,
            isVerified: false,
            timestamp: new Date(Date.now() - 25 * 60 * 1000).toISOString(),
            isOriginalPost: false,
            isReply: false,
            isQuote: false,
            isRetweet: true,
        },
        {
            id: 'mention-5',
            coinMintAddress: 'ABC123xyz456',
            tweetId: '1234567894',
            authorId: 'author5',
            username: 'solana_gems',
            profilePicture: 'https://i.pravatar.cc/150?img=55',
            tweetUrl: 'https://twitter.com/solana_gems/status/1234567894',
            tweetContent: 'Early bird gets the worm! $MOON is still undervalued IMO 💎',
            followers: 210000,
            isVerified: true,
            timestamp: new Date(Date.now() - 35 * 60 * 1000).toISOString(),
            isOriginalPost: true,
            isReply: false,
            isQuote: false,
            isRetweet: false,
        },
    ],
    'XYZ789abc123': [
        {
            id: 'mention-6',
            coinMintAddress: 'XYZ789abc123',
            tweetId: '2234567890',
            authorId: 'author6',
            username: 'crypto_analyst',
            profilePicture: 'https://i.pravatar.cc/150?img=16',
            tweetUrl: 'https://twitter.com/crypto_analyst/status/2234567890',
            tweetContent: '$STAR has solid tokenomics. Worth a look 👀',
            followers: 67000,
            isVerified: true,
            timestamp: new Date(Date.now() - 8 * 60 * 1000).toISOString(),
            isOriginalPost: true,
            isReply: false,
            isQuote: false,
            isRetweet: false,
        },
        {
            id: 'mention-7',
            coinMintAddress: 'XYZ789abc123',
            tweetId: '2234567891',
            authorId: 'author7',
            username: 'altcoin_daily',
            profilePicture: 'https://i.pravatar.cc/150?img=27',
            tweetUrl: 'https://twitter.com/altcoin_daily/status/2234567891',
            tweetContent: 'Interesting project. Researching $STAR now.',
            followers: 95000,
            isVerified: false,
            timestamp: new Date(Date.now() - 20 * 60 * 1000).toISOString(),
            isOriginalPost: false,
            isReply: true,
            isQuote: false,
            isRetweet: false,
        },
        {
            id: 'mention-8',
            coinMintAddress: 'XYZ789abc123',
            tweetId: '2234567892',
            authorId: 'author8',
            username: 'token_alerts',
            profilePicture: 'https://i.pravatar.cc/150?img=38',
            tweetUrl: 'https://twitter.com/token_alerts/status/2234567892',
            tweetContent: 'RT: $STAR has solid tokenomics. Worth a look',
            followers: 52000,
            isVerified: false,
            timestamp: new Date(Date.now() - 40 * 60 * 1000).toISOString(),
            isOriginalPost: false,
            isReply: false,
            isQuote: false,
            isRetweet: true,
        },
    ],
    'GHI789mno012': [
        {
            id: 'mention-9',
            coinMintAddress: 'GHI789mno012',
            tweetId: '3234567890',
            authorId: 'author9',
            username: 'scam_detector',
            profilePicture: 'https://i.pravatar.cc/150?img=19',
            tweetUrl: 'https://twitter.com/scam_detector/status/3234567890',
            tweetContent: 'Warning: $SCAM appears to be a rug pull. Stay away! ⚠️',
            followers: 12000,
            isVerified: false,
            timestamp: new Date(Date.now() - 3 * 60 * 1000).toISOString(),
            isOriginalPost: true,
            isReply: false,
            isQuote: false,
            isRetweet: false,
        },
    ],
};

const isDemoMode = import.meta.env.VITE_DEMO_MODE === 'true';

interface UseCoinMentionsReturn {
    mentions: CaMentionRecord[];
    loading: boolean;
    error: string | null;
    refetch: () => Promise<void>;
}

/**
 * Custom hook to fetch mention records for a specific coin address.
 *
 * @param coinAddress - The coin's mint address (Solana contract address)
 * @param hours - Number of hours to look back (default 24)
 * @returns Mention records, loading state, error state, and refetch function
 */
export const useCoinMentions = (
    coinAddress: string | null | undefined,
    hours: number = 24
): UseCoinMentionsReturn => {
    // Auth0 temporarily disabled
    // const { getAccessTokenSilently } = useAuth0();
    const [mentions, setMentions] = useState<CaMentionRecord[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    const fetchMentions = async () => {
        if (!coinAddress) {
            setMentions([]);
            setLoading(false);
            return;
        }

        // Return mock data in demo mode
        if (isDemoMode) {
            setLoading(true);
            // Simulate loading delay
            setTimeout(() => {
                const demoMentions = mockMentions[coinAddress] || [];
                setMentions(demoMentions);
                setLoading(false);
                setError(null);
            }, 500);
            return;
        }

        try {
            setLoading(true);
            setError(null);

            // Auth0 disabled - fetch without authentication
            const headers: HeadersInit = {
                'Content-Type': 'application/json',
            };

            // Auth0 temporarily disabled
            // try {
            //     const token = await getAccessTokenSilently();
            //     if (token) {
            //         headers = {
            //             ...headers,
            //             'Authorization': `Bearer ${token}`,
            //         };
            //     }
            // } catch (authError) {
            //     // User not logged in - continue without auth header
            //     console.log('No authentication available, fetching without token');
            // }

            const response = await fetch(
                API_ENDPOINTS.mentions.byCoin(coinAddress, hours),
                { headers }
            );

            if (!response.ok) {
                throw new Error(`Failed to fetch mentions: ${response.statusText}`);
            }

            const data: CaMentionRecord[] = await response.json();
            setMentions(data);
        } catch (err) {
            const errorMessage = err instanceof Error ? err.message : 'Failed to fetch mentions';
            setError(errorMessage);
            console.error('Error fetching coin mentions:', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchMentions();
    }, [coinAddress, hours]);

    return {
        mentions,
        loading,
        error,
        refetch: fetchMentions,
    };
};
