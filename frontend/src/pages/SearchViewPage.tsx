import { useParams } from 'react-router-dom';
import { useMemo } from 'react';
import styled from 'styled-components';
import { Button } from '@/components/atoms';
import { useCaMentionAlerts } from '@/providers/CaMentionAlertProvider';
import { transformCaMentionAlertsToTokens } from '@/utils/caMentionAlertTransformer';
import { useCoinMentions } from '@/hooks/useCoinMentions';
import UserIcon from '@/assets/user-icon.svg?react';
// import type { Token } from '@/types/token';

// Helper function to format follower count
const formatFollowers = (num: number): string => {
    if (num >= 1_000_000) return `${(num / 1_000_000).toFixed(1)}M`;
    if (num >= 1_000) return `${(num / 1_000).toFixed(1)}K`;
    return num.toString();
};

/**
 * ChartViewPage Component
 *
 * Displays an embedded Birdeye chart for a specific token
 */
export const SearchViewPage = () => {
    const { address } = useParams<{ address: string }>();

    // Get real-time CA mention alerts from backend
    const { alerts } = useCaMentionAlerts();

    // Transform CA mention alerts to Token format and find the token matching the address
    const token = useMemo(() => {
        const allTokens = transformCaMentionAlertsToTokens(alerts);
        return allTokens.find(t => t.contractAddress === address);
    }, [alerts, address]);

    // Fetch mentions for the current address
    const { mentions, loading: mentionsLoading, error: mentionsError } = useCoinMentions(address);

    if (!address) {
        return null;
    }

    // Build Birdeye embed URL with parameters matching the example
    const birdeyeUrl = `https://birdeye.so/tv-widget/${address}?chain=solana&viewMode=pair&chartInterval=1&chartType=candle&chartTimezone=Etc%2FUTC&chartLeftToolbar=hide&theme=dark&cssCustomProperties=--tv-color-platform-background%3A%23000000&cssCustomProperties=--tv-color-pane-background%3A%23000000&chartOverrides=paneProperties.backgroundType%3Asolid&chartOverrides=paneProperties.background%3Argba%280%2C+0%2C+0%2C+1%29&chartOverrides=mainSeriesProperties.lineStyle.color%3A%2300c572`;

    // Build Axiom URL
    const axiomUrl = `https://axiom.trade/t/${address}?chain=sol`;

    const handleOpenAxiom = () => {
        window.open(axiomUrl, '_blank', 'noopener,noreferrer');
    };

    // Build DexScreener URL
    const dexUrl = `https://dexscreener.com/solana/${address}`;

    const handleOpenDex = () => {
        window.open(dexUrl, '_blank', 'noopener,noreferrer');
    };

    const formatNumber = (num: number | null | undefined, isInteger: boolean = false): string => {
        if (num == null || num === 0) return '—';
        if (num >= 1_000_000_000) return `${(num / 1_000_000_000).toFixed(1)}B`;
        if (num >= 1_000_000) return `${(num / 1_000_000).toFixed(1)}M`;
        if (num >= 1_000) return `${(num / 1_000).toFixed(1)}K`;
        return isInteger ? Math.floor(num).toString() : num.toFixed(2);
    };

    return (
        <PageContainer>
            <HeaderSection>
                <LeftSection>
                    <TokenInfo>
                        {token?.iconUrl && <TokenLogo src={token.iconUrl} alt={token.symbol} />}
                        <TokenName>{token?.symbol || 'Unknown'}</TokenName>
                    </TokenInfo>

                    <MetricsRow>
                        <MetricsContainer>
                            <MetricGroup>
                                <MetricLabel>MCAP</MetricLabel>
                                <MetricValue>{formatNumber(token?.marketCap)}</MetricValue>
                            </MetricGroup>
                            <MetricGroup>
                                <MetricLabel>MENTIONS</MetricLabel>
                                <MetricValue>{formatNumber(token?.mentions24h, true)}</MetricValue>
                            </MetricGroup>
                            <MetricGroup>
                                <MetricLabel>HOLDERS</MetricLabel>
                                <MetricValue>{formatNumber(token?.holdersCount, true)}</MetricValue>
                            </MetricGroup>
                        </MetricsContainer>

                        <ButtonContainer>
                            <Button size="lg" onClick={handleOpenAxiom}>
                                AXIOM
                            </Button>
                            <Button size='lg' onClick={handleOpenDex}>DEX</Button>
                        </ButtonContainer>
                    </MetricsRow>
                </LeftSection>
            </HeaderSection>

            <ChartIframe
                src={birdeyeUrl}
                allowFullScreen
            />

            <MentionsSection>
                <MentionsHeader>@MENTIONS</MentionsHeader>
                {mentionsLoading && <LoadingMessage>Loading mentions...</LoadingMessage>}
                {mentionsError && <ErrorMessage>Failed to load mentions: {mentionsError}</ErrorMessage>}
                {!mentionsLoading && !mentionsError && mentions.length === 0 && (
                    <EmptyMessage>No mentions found for this token in the last 24 hours.</EmptyMessage>
                )}
                {!mentionsLoading && !mentionsError && mentions.length > 0 && (
                    <MentionsGrid>
                        {mentions.map((mention) => (
                            <MentionCard key={mention.id}>
                                <AuthorSection>
                                    <Avatar src={mention.profilePicture} alt={mention.username} />
                                    <AuthorUsername>@{mention.username}</AuthorUsername>
                                    <FollowerCount>
                                        <FollowerIcon as={UserIcon} />
                                        {formatFollowers(mention.followers)}
                                    </FollowerCount>
                                </AuthorSection>
                                <TweetText>{mention.tweetContent || 'No content available'}</TweetText>
                                <ActionButton onClick={() => window.open(mention.tweetUrl, '_blank', 'noopener,noreferrer')}>
                                    VIEW TWEET
                                </ActionButton>
                            </MentionCard>
                        ))}
                    </MentionsGrid>
                )}
            </MentionsSection>
        </PageContainer>
    );
};

// Styled Components
const PageContainer = styled.div`
    width: 100%;
    min-height: calc(100vh - 82px);
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: flex-start;
    position: relative;
    padding-top: 40px;
    overflow-y: auto;
`;

const HeaderSection = styled.div`
    width: 100%;
    max-width: 1200px;
    background-color: #000000;
    padding: 16px 24px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 16px;
`;

const LeftSection = styled.div`
    display: flex;
    flex-direction: column;
    gap: 16px;
    width: 100%;
`;

const TokenInfo = styled.div`
    display: flex;
    align-items: center;
    gap: 12px;
`;

const TokenLogo = styled.img`
    width: 40px;
    height: 40px;
    border-radius: 50%;
`;

const TokenName = styled.h1`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 24px;
    font-weight: 700;
    color: #ffffff;
    margin: 0;
    letter-spacing: 0.05em;
`;

const MetricsRow = styled.div`
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
`;

const MetricsContainer = styled.div`
    display: flex;
    gap: 48px;
    align-items: center;
`;

const MetricGroup = styled.div`
    display: flex;
    flex-direction: column;
    gap: 4px;
`;

const MetricLabel = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 11px;
    font-weight: 500;
    color: #666666;
    text-transform: uppercase;
    letter-spacing: 0.1em;
`;

const MetricValue = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 20px;
    font-weight: 600;
    color: #ffffff;
`;

const ChartIframe = styled.iframe`
    width: 100%;
    max-width: 1200px;
    height: 600px;
    border: none;
    margin-bottom: 16px;
`;

const ButtonContainer = styled.div`
    display: flex;
    gap: 4px;
`;

const MentionsSection = styled.div`
    width: 100%;
    max-width: 1200px;
    margin-top: 40px;
    margin-bottom: 40px;
`;

const MentionsHeader = styled.h2`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 16px;
    font-weight: 700;
    color: #ffffff;
    margin-bottom: 24px;
    letter-spacing: 0.05em;
`;

const MentionsGrid = styled.div`
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 16px;
`;

const MentionCard = styled.div`
    background: transparent;
    border: 1px solid ${({ theme }) => theme.colors.borderDefault};
    border-radius: 8px;
    padding: 20px;
    display: flex;
    flex-direction: column;
    gap: 16px;
    transition: .3s ease-out;
    box-shadow: 0 0 0 rgba(255, 255, 255, 0);

    &:hover {
        border: 1px solid rgba(255, 255, 255, 0.8);
        box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.04);
    }
`;

const AuthorSection = styled.div`
    display: flex;
    align-items: center;
    gap: 8px;
`;

const Avatar = styled.img`
    width: 32px;
    height: 32px;
    border-radius: 50%;
    object-fit: cover;
`;

const AuthorUsername = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 12px;
    font-weight: 600;
    color: ${({ theme }) => theme.colors.textPrimary};
`;

const FollowerCount = styled.span`
    display: flex;
    align-items: center;
    gap: 4px;
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 14px;
    color: ${({ theme }) => theme.colors.textPrimary};
`;

const FollowerIcon = styled.svg`
    width: 9px;
`;

const TweetText = styled.p`
    font-family: ${({ theme }) => theme.typography.fontFamily.secondary};
    font-size: 14px;
    line-height: 1.6;
    color: ${({ theme }) => theme.colors.textPrimary};
    margin: 0;
    min-height: 90px;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: 4;
    -webkit-box-orient: vertical;
    word-break: break-word;
    overflow-wrap: break-word;
    white-space: normal;
`;

const ActionButton = styled.button`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 12px;
    font-weight: 700;
    color: ${({ theme }) => theme.colors.textPrimary};
    background: transparent;
    border: 1px solid ${({ theme }) => theme.colors.borderDefault};
    border-radius: 4px;
    padding: 10px 16px;
    cursor: pointer;
    text-transform: uppercase;
    letter-spacing: 0.05em;
    transition: .3s ease-out;
    box-shadow: 0 0 0 rgba(255, 255, 255, 0);

    &:hover {
        border: 1px solid rgba(255, 255, 255, 0.8);
        box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.04);
    }
`;

const LoadingMessage = styled.div`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 14px;
    color: ${({ theme }) => theme.colors.textSecondary};
    text-align: center;
    padding: 40px 20px;
`;

const ErrorMessage = styled.div`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 14px;
    color: #ff4444;
    text-align: center;
    padding: 40px 20px;
`;

const EmptyMessage = styled.div`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 14px;
    color: ${({ theme }) => theme.colors.textSecondary};
    text-align: center;
    padding: 40px 20px;
`;
