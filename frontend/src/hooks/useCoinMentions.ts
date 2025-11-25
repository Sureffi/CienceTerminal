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
