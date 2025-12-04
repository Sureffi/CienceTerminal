import styled from 'styled-components';
import type { Token } from '@/types/token';
import { TokenIdentifier } from '../TokenIdentifier';

interface TokenRowProps {
    token: Token;
    onAdd?: (token: Token) => void;
    onRowClick?: (token: Token) => void;
}

/**
 * TokenRow Component
 *
 * Displays a single token's data in the screener table
 */
export const TokenRow = ({ token, onAdd, onRowClick }: TokenRowProps) => {
    const formatNumber = (num: number | null | undefined, isInteger: boolean = false): string => {
        if (num == null || num === 0) return '—';
        if (num >= 1_000_000_000) return `${(num / 1_000_000_000).toFixed(1)}B`;
        if (num >= 1_000_000) return `${(num / 1_000_000).toFixed(1)}M`;
        if (num >= 1_000) return `${(num / 1_000).toFixed(1)}K`;
        return isInteger ? Math.floor(num).toString() : num.toFixed(2);
    };

    const formatPercent = (num: number | null | undefined): string => {
        if (num == null) return '—';
        return `${num.toFixed(1)}%`;
    };

    const handleRowClick = () => {
        onRowClick?.(token);
    };

    const handleAddClick = (e: React.MouseEvent) => {
        e.stopPropagation(); // Prevent row click from triggering
        onAdd?.(token);
    };

    return (
        <RowContainer>
            <Row onClick={handleRowClick}>
                {/* Token Column */}
                <Cell $isSticky>
                    <TokenInfo>
                        {token.iconUrl && <TokenIcon src={token.iconUrl} alt={token.symbol} />}
                        <TokenIdentifier symbol={token.symbol} blockchain={token.blockchain} />
                    </TokenInfo>
                </Cell>

                {/* 24H Mentions Column */}
                <Cell>
                    <MentionsContainer>
                        <AvatarStack>
                            {token.topMentioners?.slice(0, 3).map((mentioner) => (
                                <Avatar
                                    key={mentioner.id}
                                    src={mentioner.avatarUrl}
                                    alt={mentioner.username}
                                />
                            ))}
                        </AvatarStack>
                        <MentionCount>{formatNumber(token.mentions24h, true)}</MentionCount>
                    </MentionsContainer>
                </Cell>

                {/* Chart Column - Placeholder for now */}
                {/* <Cell> */}
                {/*     <ChartPlaceholder> */}
                {/*     </ChartPlaceholder> */}
                {/* </Cell> */}

                {/* Age Column */}
                <Cell>
                    <MetricText>{token.age}</MetricText>
                </Cell>

                {/* Price Change */}
                <Cell>
                    <PriceChangeText $isPositive={(token.priceChange24h ?? 0) >= 0}>
                        {token.priceChange24h != null ? `${token.priceChange24h > 0 ? '+' : ''}${token.priceChange24h.toFixed(2)}%` : '—'}
                    </PriceChangeText>
                </Cell>

                {/* Market Cap Column */}
                <Cell>
                    <MetricText>{formatNumber(token.marketCap)}</MetricText>
                </Cell>

                {/* Volume Column */}
                <Cell>
                    <MetricText>{formatNumber(token.volume24h)}</MetricText>
                </Cell>

                {/* Liquidity Column */}
                <Cell>
                    <MetricText>{formatNumber(token.liquidity)}</MetricText>
                </Cell>

                {/* Holders Column */}
                <Cell>
                    <MetricText>{formatNumber(token.holdersCount, true)}</MetricText>
                </Cell>

                {/* Top 10 Holders % Column */}
                <Cell>
                    <MetricText>
                        {formatPercent(token.top10HoldersPercent)}
                    </MetricText>
                </Cell>

                {/* Dev Hold % Column */}
                {/* <Cell> */}
                {/*     <PercentText>{formatPercent(token.devHoldPercent)} HOLD</PercentText> */}
                {/* </Cell> */}

            </Row>

            {/* Add Button - Separate Container */}
            <AddButton onClick={handleAddClick} title="View Details">
                +
            </AddButton>
        </RowContainer>
    );
};

// Styled Components
const RowContainer = styled.div`
    display: flex;
    gap: 5px;
    margin-bottom: 5px;
`;

const Row = styled.div`
    display: grid;
    height: 44px;
    grid-template-columns: minmax(200px, 2fr) minmax(120px, 1.5fr) minmax(80px, 0.8fr) minmax(120px, 1.2fr) minmax(110px, 1.2fr) minmax(100px, 1fr) minmax(100px, 1fr) minmax(90px, 1fr) minmax(100px, 1fr);
    align-items: center;
    flex: 1;
    min-width: 1120px;
    box-shadow: 0 0 0 rgba(255, 255, 255, 0);
    transition: box-shadow .3s ease-out;
    cursor: pointer;
    background: #000000;

    &:hover {
        box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.04);
    }
 `;

const Cell = styled.div<{ $isSticky?: boolean }>`
    display: flex;
    align-items: center;
    border-top: 1px solid rgba(255, 255, 255, 0.05);
    border-bottom: 1px solid rgba(255, 255, 255, 0.05);
    height: 100%;
    transition: border-color .3s ease-out;

    ${Row}:hover & {
        border-top-color: rgba(255, 255, 255, 0.8);
        border-bottom-color: rgba(255, 255, 255, 0.8);
    }

    &:last-child {
        border-right: 1px solid rgba(255, 255, 255, 0.05);
        border-top-right-radius: 6px;
        border-bottom-right-radius: 6px;

        ${Row}:hover & {
            border-right-color: rgba(255, 255, 255, 0.8);
        }
    }

    ${({ $isSticky }) =>
        $isSticky &&
        `
        position: sticky;
        left: 0;
        background: #000000;
        z-index: 40;
        will-change: transform;
        transform: translateZ(0);
        backface-visibility: hidden;
        border-left: 1px solid rgba(255, 255, 255, 0.05);
        border-top-left-radius: 6px;
        border-bottom-left-radius: 6px;
    `}

    ${Row}:hover & {
        ${({ $isSticky }) =>
        $isSticky &&
        `
            border-left-color: rgba(255, 255, 255, 0.8);
        `}
    }
`;

const TokenInfo = styled.div`
    align-items: center;
    display: flex;
    position: relative;
    gap: 16px;
`;

const TokenIcon = styled.img`
    width: 32px;
    height: 32px;
    border-radius: 3px;
    margin: 5px;
`;

const MentionsContainer = styled.div`
    display: flex;
    align-items: center;
    gap: 10px;
`;

const AvatarStack = styled.div`
    display: flex;
    align-items: center;
`;

const Avatar = styled.img`
    width: 28px;
    border-radius: 50%;
    border: 2px solid black;
    margin-left: -15px;
    object-fit: cover;

    &:first-child {
        margin-left: 0;
    }
`;

const MentionCount = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    color: ${({ theme }) => theme.colors.textPrimary};
`;

// const ChartPlaceholder = styled.div`
//     width: 54px;
//     height: 21px;
//     background: #86EFAC;
//     border-radius: ${({ theme }) => theme.borderRadius.sm};
// `;

const MetricText = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    color: ${({ theme }) => theme.colors.textPrimary};
`;

// const PercentText = styled.span<{ $isHighRisk?: boolean }>`
//     font-family: ${({ theme }) => theme.typography.fontFamily.mono};
//     font-size: 12px;
//     color: ${({ $isHighRisk }) => ($isHighRisk ? '#FF6868' : '#A2FF68')};
// `;

const PriceChangeText = styled.span<{ $isPositive: boolean }>`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    color: ${({ $isPositive }) => ($isPositive ? '#A2FF68' : '#FF6868')};
`;

const AddButton = styled.button`
    height: 44px;
    min-width: 44px;
    border-radius: 6px;
    border: 1px solid rgba(255, 255, 255, 0.05);
    background: transparent;
    color: ${({ theme }) => theme.colors.textPrimary};
    font-size: 20px;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: .3s ease-out;
    box-shadow: 0 0 0 rgba(255, 255, 255, 0);

    &:hover {
        border: 1px solid rgba(255, 255, 255, 0.8);
        box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.04);
    }
`;
