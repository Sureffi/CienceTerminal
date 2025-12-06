import { Drawer } from 'vaul';
import styled from 'styled-components';
import { Button } from '@/components/atoms';
import type { Token } from '@/types/token';
import XIcon from '@/assets/x.svg?react';
import UserIcon from '@/assets/user-icon.svg?react';
import { useCoinMentions } from '@/hooks/useCoinMentions';
import { DIMENSIONS, SPACING, Z_INDEX, TYPOGRAPHY, MOBILE_BREAKPOINT } from './constants';

interface TokenDetailsDrawerProps {
    isOpen: boolean;
    onClose: () => void;
    token: Token | null;
}

/**
 * Helper function to format numbers with K/M/B suffixes
 * Replaces both formatFollowers and formatNumber with a single utility
 */
const formatNumber = (num: number | null | undefined, isInteger: boolean = false): string => {
    if (num == null || num === 0) return '—';
    if (num >= 1_000_000_000) return `${(num / 1_000_000_000).toFixed(1)}B`;
    if (num >= 1_000_000) return `${(num / 1_000_000).toFixed(1)}M`;
    if (num >= 1_000) return `${(num / 1_000).toFixed(1)}K`;
    return isInteger ? Math.floor(num).toString() : num.toFixed(2);
};

/**
 * Helper function to open external links
 */
const openExternalLink = (url: string) => {
    window.open(url, '_blank', 'noopener,noreferrer');
};

/**
 * TokenDetailsDrawer Component
 *
 * Displays token details in a bottom drawer using Vaul.
 * Includes token metrics, price chart, and recent Twitter mentions.
 *
 * Responsive Design Pattern (following Header and TokenTable):
 * - Uses constants from ./constants for maintainability
 * - Mobile breakpoint (768px) for consistent responsive behavior
 * - Touch optimization for better mobile UX
 * - GPU acceleration on mobile for smooth performance
 */
export const TokenDetailsDrawer = ({ isOpen, onClose, token }: TokenDetailsDrawerProps) => {
    // Fetch mentions for the current token
    const { mentions, loading: mentionsLoading, error: mentionsError } = useCoinMentions(token?.contractAddress);

    if (!token || !token.contractAddress) {
        return null;
    }

    // Build external URLs
    const birdeyeUrl = `https://birdeye.so/tv-widget/${token.contractAddress}?chain=solana&viewMode=pair&chartInterval=1&chartType=candle&chartTimezone=Etc%2FUTC&chartLeftToolbar=hide&theme=dark&cssCustomProperties=--tv-color-platform-background%3A%23000000&cssCustomProperties=--tv-color-pane-background%3A%23000000&chartOverrides=paneProperties.backgroundType%3Asolid&chartOverrides=paneProperties.background%3Argba%280%2C+0%2C+0%2C+1%29&chartOverrides=mainSeriesProperties.lineStyle.color%3A%2300c572`;
    const axiomUrl = `https://axiom.trade/t/${token.contractAddress}?chain=sol`;
    const dexUrl = `https://dexscreener.com/solana/${token.contractAddress}`;

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
                                    <ResponsiveExternalButton size="lg" onClick={() => openExternalLink(axiomUrl)}>
                                        AXIOM
                                    </ResponsiveExternalButton>
                                    <ResponsiveExternalButton size="lg" onClick={() => openExternalLink(dexUrl)}>
                                        DEX
                                    </ResponsiveExternalButton>
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
                                                    {formatNumber(mention.followers, true)}
                                                </FollowerCount>
                                            </AuthorSection>
                                            <TweetText>{mention.tweetContent || 'No content available'}</TweetText>
                                            <ActionButton onClick={() => openExternalLink(mention.tweetUrl)}>
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

/**
 * Styled Components
 *
 * Responsive Design Pattern (following Header, TokenTable, Tabs):
 * - Uses constants from ./constants for maintainability
 * - Mobile breakpoint (768px) for consistent responsive behavior
 * - Touch optimization (touch-action) for better mobile UX
 * - GPU acceleration (translateZ) on mobile for smooth performance
 * - Theme values for colors, transitions, and typography
 * - Hover media queries to prevent hover effects on touch devices
 */
const DrawerOverlay = styled(Drawer.Overlay)`
    position: fixed;
    inset: 0;
    background-color: ${({ theme }) => theme.colors.bgTransparent || 'rgba(0, 0, 0, 0.5)'};
    z-index: ${Z_INDEX.overlay};
`;

const DrawerContent = styled(Drawer.Content)`
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    margin: 0 auto;
    height: ${DIMENSIONS.drawer.heightDesktop};
    max-width: ${DIMENSIONS.drawer.maxWidthDesktop}px;
    background: ${({ theme }) => theme.colors.bgDark};
    border-top-left-radius: ${DIMENSIONS.drawer.borderRadius}px;
    border-top-right-radius: ${DIMENSIONS.drawer.borderRadius}px;
    border: 1px solid ${({ theme }) => theme.colors.borderGhost};
    border-bottom: none;
    z-index: ${Z_INDEX.content};
    display: flex;
    flex-direction: column;

    /* Mobile optimizations */
    @media (max-width: 768px) {
        height: ${DIMENSIONS.drawer.heightMobile};
        /* GPU acceleration for smooth scrolling on mobile */
        transform: translateZ(0);
    }
`;

const DrawerHeader = styled.div`
    position: relative;
    display: flex;
    justify-content: center;
    align-items: center;
    padding: ${SPACING.padding.drawerHeaderDesktop};
    flex-shrink: 0;

    @media (max-width: 768px) {
        padding: ${SPACING.padding.drawerHeaderMobile};
    }
`;

const DrawerHandle = styled.div`
    width: ${DIMENSIONS.handle.width}px;
    height: ${DIMENSIONS.handle.height}px;
    background: ${({ theme }) => theme.colors.borderDefault};
    border-radius: ${DIMENSIONS.handle.borderRadius}px;
    flex-shrink: 0;
`;

const CloseButton = styled.button`
    position: absolute;
    top: 12px;
    right: 16px;
    width: ${DIMENSIONS.closeButton.size}px;
    height: ${DIMENSIONS.closeButton.size}px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: transparent;
    border: none;
    cursor: pointer;
    transition: ${({ theme }) => theme.transitions.fast};
    padding: 0;

    /* Touch optimization - prevents double-tap zoom delay on mobile */
    touch-action: manipulation;

    svg {
        width: ${DIMENSIONS.closeButton.iconSize}px;
        height: ${DIMENSIONS.closeButton.iconSize}px;
    }

    /* Hover effects only on devices with hover capability */
    @media (hover: hover) {
        &:hover {
            opacity: 0.7;
        }
    }

    /* Active state for touch feedback */
    &:active {
        transform: scale(0.95);
    }
`;

const ContentWrapper = styled.div`
    flex: 1;
    overflow-y: auto;
    padding: ${SPACING.padding.contentMobile};
    display: flex;
    flex-direction: column;
    align-items: center;

    /* Touch optimization for smooth scrolling */
    -webkit-overflow-scrolling: touch;
    overscroll-behavior: contain;

    @media (min-width: 768px) {
        padding: ${SPACING.padding.contentDesktop};
    }
`;

const HeaderSection = styled.div`
    width: 100%;
    max-width: ${DIMENSIONS.drawer.maxWidthContent}px;
    background-color: ${({ theme }) => theme.colors.bgDark};
    padding: ${SPACING.padding.headerSectionMobile};
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: ${SPACING.margin.chartBottom}px;

    @media (min-width: 768px) {
        padding: ${SPACING.padding.headerSectionDesktop};
    }
`;

const ResponsiveGrid = styled.div`
    display: grid;
    width: 100%;
    gap: ${SPACING.gap.large}px;

    /* Mobile: Token + Buttons on top, Metrics below */
    grid-template-columns: 1fr auto;
    grid-template-areas:
        "token buttons"
        "metrics metrics";
    align-items: center;

    @media (min-width: 768px) {
        /* Tablet+: Token on top, Metrics + Buttons on bottom row */
        grid-template-columns: 1fr auto;
        grid-template-areas:
            "token token"
            "metrics buttons";
        align-items: start;
    }
`;

const TokenInfo = styled.div`
    grid-area: token;
    display: flex;
    align-items: center;
    gap: ${SPACING.gap.mediumLarge}px;
`;

const TokenLogo = styled.img`
    width: ${DIMENSIONS.logo.sizeDesktop}px;
    height: ${DIMENSIONS.logo.sizeDesktop}px;
    border-radius: 50%;

    @media (max-width: 768px) {
        width: ${DIMENSIONS.logo.sizeMobile}px;
        height: ${DIMENSIONS.logo.sizeMobile}px;
    }
`;

const TokenName = styled.h1`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${TYPOGRAPHY.tokenName.sizeMobile}px;
    font-weight: ${TYPOGRAPHY.tokenName.weight};
    color: ${({ theme }) => theme.colors.textPrimary};
    margin: 0;
    letter-spacing: 0.05em;

    @media (min-width: 768px) {
        font-size: ${TYPOGRAPHY.tokenName.sizeDesktop}px;
    }
`;

const MetricsContainer = styled.div`
    grid-area: metrics;
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: ${SPACING.gap.extraLarge}px;
    align-items: center;

    @media (min-width: 768px) {
        gap: ${SPACING.gap.huge}px;
    }
`;

const MetricGroup = styled.div`
    display: flex;
    flex-direction: column;
    gap: ${SPACING.gap.small}px;
`;

const MetricLabel = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${TYPOGRAPHY.metricLabel.size}px;
    font-weight: ${TYPOGRAPHY.metricLabel.weight};
    color: ${({ theme }) => theme.colors.textMuted};
    text-transform: uppercase;
    letter-spacing: 0.1em;
`;

const MetricValue = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${TYPOGRAPHY.metricValue.sizeMobile}px;
    font-weight: ${TYPOGRAPHY.metricValue.weight};
    color: ${({ theme }) => theme.colors.textPrimary};

    @media (min-width: 768px) {
        font-size: ${TYPOGRAPHY.metricValue.sizeDesktop}px;
    }
`;

const ChartIframe = styled.iframe`
    width: 100%;
    max-width: ${DIMENSIONS.drawer.maxWidthContent}px;
    height: ${DIMENSIONS.chart.heightMobile}px;
    min-height: ${DIMENSIONS.chart.heightMobile}px;
    border: none;
    margin-bottom: ${SPACING.margin.chartBottom}px;
    flex-shrink: 0;

    @media (min-width: 768px) {
        height: ${DIMENSIONS.chart.heightTablet}px;
        min-height: ${DIMENSIONS.chart.heightTablet}px;
    }

    @media (min-width: 1024px) {
        height: ${DIMENSIONS.chart.heightDesktop}px;
        min-height: ${DIMENSIONS.chart.heightDesktop}px;
    }
`;

const ButtonContainer = styled.div`
    grid-area: buttons;
    display: flex;
    gap: ${SPACING.gap.small}px;
    flex-shrink: 0;

    /* Touch optimization - prevents double-tap zoom delay on mobile */
    touch-action: manipulation;
`;

const ResponsiveExternalButton = styled(Button)`
    /* Mobile: smaller font and tighter padding for space efficiency */
    @media (max-width: 768px) {
        font-size: 12px;
        padding-left: 3px;
        padding-right: 3px;
        padding-top: 0px;
        padding-bottom: 0px;
        height: 28px;
    }

    @media (min-width: 769px) {
        font-size: ${TYPOGRAPHY.externalButton.sizeDesktop}px !important;
        padding: ${TYPOGRAPHY.externalButton.paddingDesktop} !important;
    }
`;

const MentionsSection = styled.div`
    width: 100%;
    max-width: ${DIMENSIONS.drawer.maxWidthContent}px;
    margin-top: ${SPACING.margin.mentionsSectionTop}px;
    margin-bottom: ${SPACING.margin.mentionsSectionBottom}px;
`;

const MentionsHeader = styled.h2`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${TYPOGRAPHY.mentionsHeader.size}px;
    font-weight: ${TYPOGRAPHY.mentionsHeader.weight};
    color: ${({ theme }) => theme.colors.textPrimary};
    margin-bottom: ${SPACING.margin.mentionsHeaderBottom}px;
    letter-spacing: 0.05em;
`;

const MentionsGrid = styled.div`
    display: grid;
    grid-template-columns: 1fr;
    gap: ${SPACING.gap.large}px;

    @media (min-width: 768px) {
        grid-template-columns: repeat(2, 1fr);
    }

    @media (min-width: 1024px) {
        grid-template-columns: repeat(3, 1fr);
    }
`;

const MentionCard = styled.div`
    background: transparent;
    border: 1px solid ${({ theme }) => theme.colors.borderDefault};
    border-radius: ${DIMENSIONS.card.borderRadius}px;
    padding: ${SPACING.padding.cardMobile};
    display: flex;
    flex-direction: column;
    gap: ${SPACING.gap.cardMobile}px;
    transition: ${({ theme }) => theme.transitions.normal};
    box-shadow: 0 0 0 rgba(255, 255, 255, 0);

    /* Touch optimization - prevents double-tap zoom delay on mobile */
    touch-action: manipulation;

    @media (min-width: ${MOBILE_BREAKPOINT}) {
        padding: ${SPACING.padding.cardDesktop};
        gap: ${SPACING.gap.cardDesktop}px;
    }

    /* Hover effects only on devices with hover capability */
    @media (hover: hover) {
        &:hover {
            border: 1px solid ${({ theme }) => theme.colors.textPrimary};
            box-shadow: 0 0 0 3px ${({ theme }) => theme.colors.borderGhost};
        }
    }

    /* Active state for touch feedback on mobile */
    &:active {
        transform: scale(0.99);
        border: 1px solid ${({ theme }) => theme.colors.textPrimary};
    }

    @media (max-width: 768px) {
        /* GPU acceleration for smooth animations on mobile */
        transform: translateZ(0);
    }
`;

const AuthorSection = styled.div`
    display: flex;
    align-items: center;
    gap: ${SPACING.gap.authorSectionMobile}px;

    @media (min-width: ${MOBILE_BREAKPOINT}) {
        gap: ${SPACING.gap.authorSectionDesktop}px;
    }
`;

const Avatar = styled.img`
    width: ${DIMENSIONS.avatar.sizeMobile}px;
    height: ${DIMENSIONS.avatar.sizeMobile}px;
    border-radius: 50%;
    object-fit: cover;

    @media (min-width: ${MOBILE_BREAKPOINT}) {
        width: ${DIMENSIONS.avatar.sizeDesktop}px;
        height: ${DIMENSIONS.avatar.sizeDesktop}px;
    }
`;

const AuthorUsername = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${TYPOGRAPHY.authorUsername.sizeMobile}px;
    font-weight: ${TYPOGRAPHY.authorUsername.weight};
    color: ${({ theme }) => theme.colors.textPrimary};

    @media (min-width: ${MOBILE_BREAKPOINT}) {
        font-size: ${TYPOGRAPHY.authorUsername.sizeDesktop}px;
    }
`;

const FollowerCount = styled.span`
    display: flex;
    align-items: center;
    gap: ${SPACING.gap.small}px;
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${TYPOGRAPHY.followerCount.sizeMobile}px;
    color: ${({ theme }) => theme.colors.textPrimary};

    @media (min-width: ${MOBILE_BREAKPOINT}) {
        font-size: ${TYPOGRAPHY.followerCount.sizeDesktop}px;
    }
`;

const FollowerIcon = styled.svg`
    width: ${DIMENSIONS.icon.followerIconMobile}px;

    @media (min-width: ${MOBILE_BREAKPOINT}) {
        width: ${DIMENSIONS.icon.followerIconDesktop}px;
    }
`;

const TweetText = styled.p`
    font-family: ${({ theme }) => theme.typography.fontFamily.secondary};
    font-size: ${TYPOGRAPHY.tweetText.sizeMobile}px;
    line-height: ${TYPOGRAPHY.tweetText.lineHeight};
    color: ${({ theme }) => theme.colors.textPrimary};
    margin: 0;
    min-height: ${TYPOGRAPHY.tweetText.minHeightMobile}px;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: ${TYPOGRAPHY.tweetText.clampMobile};
    -webkit-box-orient: vertical;
    word-break: break-word;
    overflow-wrap: break-word;
    white-space: normal;

    @media (min-width: ${MOBILE_BREAKPOINT}) {
        font-size: ${TYPOGRAPHY.tweetText.sizeDesktop}px;
        min-height: ${TYPOGRAPHY.tweetText.minHeightTablet}px;
        -webkit-line-clamp: ${TYPOGRAPHY.tweetText.clampTablet};
    }
`;

const ActionButton = styled.button`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${TYPOGRAPHY.actionButton.sizeMobile}px;
    font-weight: ${TYPOGRAPHY.actionButton.weight};
    color: ${({ theme }) => theme.colors.textPrimary};
    background: transparent;
    border: 1px solid ${({ theme }) => theme.colors.borderDefault};
    border-radius: ${DIMENSIONS.card.buttonBorderRadius}px;
    padding: ${SPACING.padding.actionButtonMobile};
    height: ${DIMENSIONS.button.heightMobile}px;
    cursor: pointer;
    text-transform: uppercase;
    letter-spacing: 0.05em;
    transition: ${({ theme }) => theme.transitions.normal};
    box-shadow: 0 0 0 rgba(255, 255, 255, 0);

    /* Touch optimization - prevents double-tap zoom delay on mobile */
    touch-action: manipulation;

    @media (min-width: ${MOBILE_BREAKPOINT}) {
        font-size: ${TYPOGRAPHY.actionButton.sizeDesktop}px;
        padding: ${SPACING.padding.actionButtonDesktop};
        height: ${DIMENSIONS.button.heightDesktop}px;
    }

    /* Hover effects only on devices with hover capability */
    @media (hover: hover) {
        &:hover {
            border: 1px solid ${({ theme }) => theme.colors.textPrimary};
            box-shadow: 0 0 0 3px ${({ theme }) => theme.colors.borderGhost};
        }
    }

    /* Active state for touch feedback */
    &:active {
        transform: scale(0.98);
    }
`;

const LoadingMessage = styled.div`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${TYPOGRAPHY.message.size}px;
    color: ${({ theme }) => theme.colors.textSecondary};
    text-align: center;
    padding: ${SPACING.padding.message};
`;

const ErrorMessage = styled.div`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${TYPOGRAPHY.message.size}px;
    color: ${({ theme }) => theme.colors.textGhost};
    text-align: center;
    padding: ${SPACING.padding.message};
`;

const EmptyMessage = styled.div`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${TYPOGRAPHY.message.size}px;
    color: ${({ theme }) => theme.colors.textSecondary};
    text-align: center;
    padding: ${SPACING.padding.message};
`;
