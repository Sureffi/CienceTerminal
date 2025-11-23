import styled from 'styled-components';
import { Button } from '@/components/atoms';
import { TokenCard } from '@/components/molecules';
import type { Token } from '@/types/token';
import UserIcon from '@/assets/user-icon.svg?react';
interface TweetCardProps {
    token?: Token;
    useMockData?: boolean;
}

// Mock data for preview
const MOCK_TOKEN: Token = {
    id: '1',
    symbol: 'SPX6900',
    blockchain: 'SOL',
    iconUrl: 'https://raw.githubusercontent.com/solana-labs/token-list/main/assets/mainnet/So11111111111111111111111111111111111111112/logo.png',
    mentions24h: 2435,
    priceHistory: [100, 120, 115, 140, 135, 150, 145],
    age: '3H',
    marketCap: 396_000,
    volume24h: 2_400_000,
    liquidity: 64_000,
    holdersCount: 2_300,
    top10HoldersPercent: 25,
    devHoldPercent: 5,
    tweet: {
        id: 'tweet1',
        author: {
            username: 'sureffi',
            followers: 469_600,
            avatarUrl: 'https://i.pravatar.cc/150?img=1',
        },
        text: '...have the gift to make meaning with each other. SPX6900 is, by far and away, the highest meaning generating coin per holder in cr...',
        createdAt: new Date(),
        tweetUrl: 'https://twitter.com/sureffi/status/1234567890',
    },
};

const formatNumber = (num: number): string => {
    if (num >= 1_000_000) return `${(num / 1_000_000).toFixed(1)}M`;
    if (num >= 1_000) return `${(num / 1_000).toFixed(1)}K`;
    return num.toString();
};

export const TweetCard = ({ token, useMockData = false }: TweetCardProps) => {
    const displayToken = useMockData ? MOCK_TOKEN : token;

    if (!displayToken) return null;

    const tweet = displayToken.tweet;
    if (!tweet) return null;

    // TODO(human): Add highlighted token symbol logic
    const highlightTokenSymbol = (text: string, _symbol: string): React.ReactNode => {
        // This should split the text and wrap the token symbol in a styled span
        return text;
    };

    const handleViewTweet = () => {
        if (tweet.tweetUrl) {
            window.open(tweet.tweetUrl, '_blank', 'noopener,noreferrer');
        }
    };

    return (
        <Container>
            {/* Author Section */}
            <AuthorSection>
                <AuthorInfo>
                    <Avatar src={tweet.author.avatarUrl} alt={tweet.author.username} />
                    <Username>@{tweet.author.username}</Username>
                    <FollowerCount>
                        <FollowerIcon as={UserIcon} />
                        {formatNumber(tweet.author.followers)}
                    </FollowerCount>
                </AuthorInfo>
            </AuthorSection>

            {/* Tweet Text */}
            <TweetText>{highlightTokenSymbol(tweet.text, displayToken.symbol)}</TweetText>

            {/* Token Card */}
            <TokenCard token={displayToken} />

            {/* Token Info Card */}
            <TokenInfoCard>
                {/* Stats Grid */}
                <StatsGrid>
                    <StatRow>
                        <StatLabel>24H MENTIONS</StatLabel>
                        <StatValue>{displayToken.mentions24h}</StatValue>
                    </StatRow>
                    <StatRow>
                        <StatLabel>TOKEN AGE</StatLabel>
                        <StatValue>{displayToken.age}</StatValue>
                    </StatRow>
                    <StatRow>
                        <StatLabel>LIQUIDITY</StatLabel>
                        <StatValue>{formatNumber(displayToken.liquidity)}</StatValue>
                    </StatRow>
                    <StatRow>
                        <StatLabel>HOLDERS</StatLabel>
                        <StatValue>{formatNumber(displayToken.holdersCount)}</StatValue>
                    </StatRow>
                </StatsGrid>
            </TokenInfoCard>

            {/* Action Buttons */}
            <ActionButtons>
                <Button size="lg" onClick={handleViewTweet}>VIEW TWEET</Button>
                <Button size="lg">AXIOM</Button>
                <Button size="lg">DEX</Button>
            </ActionButtons>
        </Container>
    );
};

// Styled Components
const Container = styled.div`
    background: transparent;
    border: 1px solid ${({ theme }) => theme.colors.borderDefault};
    border-radius: 8px;
    padding: 20px;
    box-sizing: border-box;
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
    gap: 12px;
    margin-bottom: 16px;
`;

const Avatar = styled.img`
    width: 32px;
    height: 32px;
    border-radius: 50%;
    object-fit: cover;
`;

const AuthorInfo = styled.div`
    display: flex;
    align-items: center;
    flex-direction: row;
    gap: 8px;
`;

const Username = styled.div`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 12px;
    font-weight: 600;
    color: ${({ theme }) => theme.colors.textPrimary};
`;

const FollowerCount = styled.div`
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
    margin-bottom: 25px;
    width: 320px;
    height: 54px;
    overflow: hidden;
    text-overflow: ellipsis;
    display: -webkit-box;
    -webkit-line-clamp: 3;
    -webkit-box-orient: vertical;
`;

const TokenInfoCard = styled.div`
    background: transparent;
    border: 1px solid rgba(255, 255, 255, 0.10);
    border-radius: 4px;
    padding: 15px;
    display: flex;
    flex-direction: column;
    margin-bottom: 10px;
    margin-top: 4px;
`;

const StatsGrid = styled.div`
    display: grid;
    gap: 12px;
`;

const StatRow = styled.div`
    display: flex;
    justify-content: space-between;
    align-items: center;
`;

const StatLabel = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 12px;
    font-weight: ${({ theme }) => theme.typography.fontWeight.medium};
    color: ${({ theme }) => theme.colors.textGhost};
`;

const StatValue = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 12px;
    font-weight: ${({ theme }) => theme.typography.fontWeight.bold};
    color: ${({ theme }) => theme.colors.textPrimary};
`;

const ActionButtons = styled.div`
    display: flex;
    gap: 4px;
`;
