import { Drawer } from 'vaul';
import styled from 'styled-components';
import { Button } from '@/components/atoms';
import type { Token } from '@/types/token';
import XIcon from '@/assets/x.svg?react';
import UserIcon from '@/assets/user-icon.svg?react';
import { useCoinMentions } from '@/hooks/useCoinMentions';

interface TokenDetailsDrawerProps {
    isOpen: boolean;
    onClose: () => void;
    token: Token | null;
}

// Helper function to format follower count
const formatFollowers = (num: number): string => {
    if (num >= 1_000_000) return `${(num / 1_000_000).toFixed(1)}M`;
    if (num >= 1_000) return `${(num / 1_000).toFixed(1)}K`;
    return num.toString();
};

/**
 * TokenDetailsDrawer Component
 *
 * Displays token details in a bottom drawer using Vaul
 */
export const TokenDetailsDrawer = ({ isOpen, onClose, token }: TokenDetailsDrawerProps) => {
    // Fetch mentions for the current token
    const { mentions, loading: mentionsLoading, error: mentionsError } = useCoinMentions(token?.contractAddress);

    if (!token || !token.contractAddress) {
        return null;
    }

    // Build Birdeye embed URL with parameters matching the example
    const birdeyeUrl = `https://birdeye.so/tv-widget/${token.contractAddress}?chain=solana&viewMode=pair&chartInterval=1&chartType=candle&chartTimezone=Etc%2FUTC&chartLeftToolbar=hide&theme=dark&cssCustomProperties=--tv-color-platform-background%3A%23000000&cssCustomProperties=--tv-color-pane-background%3A%23000000&chartOverrides=paneProperties.backgroundType%3Asolid&chartOverrides=paneProperties.background%3Argba%280%2C+0%2C+0%2C+1%29&chartOverrides=mainSeriesProperties.lineStyle.color%3A%2300c572`;

    // Build Axiom URL
    const axiomUrl = `https://axiom.trade/t/${token.contractAddress}?chain=sol`;

    const handleOpenAxiom = () => {
        window.open(axiomUrl, '_blank', 'noopener,noreferrer');
    };

    // Build DexScreener URL
    const dexUrl = `https://dexscreener.com/solana/${token.contractAddress}`;

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
        <Drawer.Root open={isOpen} onOpenChange={(open) => !open && onClose()}>
            <Drawer.Portal>
                <DrawerOverlay />
                <DrawerContent>
                    <DrawerHeader>
                        <DrawerHandle />
                        <CloseButton onClick={onClose}>
                            <XIcon />
                        </CloseButton>
                    </DrawerHeader>
                    <ContentWrapper>
                        <HeaderSection>
                            <ResponsiveGrid>
                                <TokenInfo>
                                    {token.iconUrl && <TokenLogo src={token.iconUrl} alt={token.symbol} />}
                                    <TokenName>{token.symbol}</TokenName>
                                </TokenInfo>

                                <MetricsContainer>
                                    <MetricGroup>
                                        <MetricLabel>MCAP</MetricLabel>
                                        <MetricValue>{formatNumber(token.marketCap)}</MetricValue>
                                    </MetricGroup>
                                    <MetricGroup>
                                        <MetricLabel>MENTIONS</MetricLabel>
                                        <MetricValue>{formatNumber(token.mentions24h, true)}</MetricValue>
                                    </MetricGroup>
                                    <MetricGroup>
                                        <MetricLabel>HOLDERS</MetricLabel>
                                        <MetricValue>{formatNumber(token.holdersCount, true)}</MetricValue>
                                    </MetricGroup>
                                </MetricsContainer>

                                <ButtonContainer>
                                    <Button size="lg" onClick={handleOpenAxiom}>
                                        AXIOM
                                    </Button>
                                    <Button size='lg' onClick={handleOpenDex}>DEX</Button>
                                </ButtonContainer>
                            </ResponsiveGrid>
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
                    </ContentWrapper>
                </DrawerContent>
            </Drawer.Portal>
        </Drawer.Root>
    );
};

// Styled Components
const DrawerOverlay = styled(Drawer.Overlay)`
    position: fixed;
    inset: 0;
    background-color: rgba(0, 0, 0, 0.5);
    z-index: 1000;
`;

const DrawerContent = styled(Drawer.Content)`
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    margin: 0 auto;
    height: 90vh;
    max-width: 1500px;
    background: #000000;
    border-top-left-radius: 16px;
    border-top-right-radius: 16px;
    border: 1px solid rgba(255, 255, 255, 0.1);
    border-bottom: none;
    z-index: 1001;
    display: flex;
    flex-direction: column;
`;

const DrawerHeader = styled.div`
    position: relative;
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 12px 16px;
    flex-shrink: 0;
`;

const DrawerHandle = styled.div`
    width: 48px;
    height: 4px;
    background: rgba(255, 255, 255, 0.2);
    border-radius: 2px;
    flex-shrink: 0;
`;

const CloseButton = styled.button`
    position: absolute;
    top: 12px;
    right: 16px;
    width: 32px;
    height: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: transparent;
    border: none;
    cursor: pointer;
    transition: all 0.2s ease;
    padding: 0;


    svg {
        width: 14px;
        height: 14px;
    }
`;

const ContentWrapper = styled.div`
    flex: 1;
    overflow-y: auto;
    padding: 12px 16px;
    display: flex;
    flex-direction: column;
    align-items: center;

    ${({ theme }) => theme.media.tablet`
        padding: 16px 24px;
    `}
`;

const HeaderSection = styled.div`
    width: 100%;
    max-width: 1200px;
    background-color: #000000;
    padding: 12px 16px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 16px;

    ${({ theme }) => theme.media.tablet`
        padding: 16px 24px;
    `}
`;

const ResponsiveGrid = styled.div`
    display: grid;
    width: 100%;
    gap: 16px;

    /* Mobile: Token + Buttons on top, Metrics below */
    grid-template-columns: 1fr auto;
    grid-template-areas:
        "token buttons"
        "metrics metrics";
    align-items: center;

    ${({ theme }) => theme.media.tablet`
        /* Tablet+: Token on top, Metrics + Buttons on bottom row */
        grid-template-columns: 1fr auto;
        grid-template-areas:
            "token token"
            "metrics buttons";
        align-items: start;
    `}
`;

const TokenInfo = styled.div`
    grid-area: token;
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
    font-size: 20px;
    font-weight: 700;
    color: #ffffff;
    margin: 0;
    letter-spacing: 0.05em;

    ${({ theme }) => theme.media.tablet`
        font-size: 24px;
    `}
`;

const MetricsContainer = styled.div`
    grid-area: metrics;
    display: flex;
    gap: 24px;
    align-items: center;

    ${({ theme }) => theme.media.tablet`
        gap: 48px;
    `}
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
    font-size: 16px;
    font-weight: 600;
    color: #ffffff;

    ${({ theme }) => theme.media.tablet`
        font-size: 20px;
    `}
`;

const ChartIframe = styled.iframe`
    width: 100%;
    max-width: 1200px;
    height: 400px;
    min-height: 400px;
    border: none;
    margin-bottom: 16px;
    flex-shrink: 0;

    ${({ theme }) => theme.media.tablet`
        height: 500px;
        min-height: 500px;
    `}

    ${({ theme }) => theme.media.desktop`
        height: 600px;
        min-height: 600px;
    `}
`;

const ButtonContainer = styled.div`
    grid-area: buttons;
    display: flex;
    gap: 4px;
    flex-shrink: 0;
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
    grid-template-columns: 1fr;
    gap: 16px;

    ${({ theme }) => theme.media.tablet`
        grid-template-columns: repeat(2, 1fr);
    `}

    ${({ theme }) => theme.media.desktop`
        grid-template-columns: repeat(3, 1fr);
    `}
`;

const MentionCard = styled.div`
    background: transparent;
    border: 1px solid ${({ theme }) => theme.colors.borderDefault};
    border-radius: 8px;
    padding: 16px;
    display: flex;
    flex-direction: column;
    gap: 12px;
    transition: .3s ease-out;
    box-shadow: 0 0 0 rgba(255, 255, 255, 0);

    ${({ theme }) => theme.media.tablet`
        padding: 20px;
        gap: 16px;
    `}

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
    min-height: 67px;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: 3;
    -webkit-box-orient: vertical;
    word-break: break-word;
    overflow-wrap: break-word;
    white-space: normal;

    ${({ theme }) => theme.media.tablet`
        min-height: 90px;
        -webkit-line-clamp: 4;
    `}
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
