import styled from 'styled-components';
import type { Token } from '@/types/token';
import { TokenIdentifier } from '../TokenIdentifier';
import { formatNumber, formatPercent, formatPriceChange } from '@/utils/formatters';
import { GRID_COLUMNS, MIN_TABLE_WIDTH, Z_INDEX, DIMENSIONS, stickyColumnStyles } from '@/components/organisms/TokenTable/constants';

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

                {/* Age Column */}
                <Cell>
                    <MetricText>{token.age}</MetricText>
                </Cell>

                {/* Price Change */}
                <Cell>
                    <PriceChangeText $isPositive={(token.priceChange24h ?? 0) >= 0}>
                        {formatPriceChange(token.priceChange24h)}
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
    height: ${DIMENSIONS.rowHeight}px;
    grid-template-columns: ${GRID_COLUMNS};
    align-items: center;
    flex: 1;
    min-width: ${MIN_TABLE_WIDTH};
    transition: box-shadow ${({ theme }) => theme.transitions.normal};
    cursor: pointer;
    background: ${({ theme }) => theme.colors.background};

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
    transition: border-color ${({ theme }) => theme.transitions.normal};

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
        ${stickyColumnStyles(Z_INDEX.stickyColumn)}
        background: ${({ theme }) => theme.colors.background};
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
    width: ${DIMENSIONS.tokenIconSize}px;
    height: ${DIMENSIONS.tokenIconSize}px;
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
    width: ${DIMENSIONS.avatarSize}px;
    height: ${DIMENSIONS.avatarSize}px;
    border-radius: 50%;
    border: 2px solid ${({ theme }) => theme.colors.background};
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

const MetricText = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    color: ${({ theme }) => theme.colors.textPrimary};
`;

const PriceChangeText = styled.span<{ $isPositive: boolean }>`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    color: ${({ theme, $isPositive }) => ($isPositive ? theme.colors.pricePositive : theme.colors.priceNegative)};
`;

const AddButton = styled.button`
    height: ${DIMENSIONS.actionButtonSize}px;
    min-width: ${DIMENSIONS.actionButtonSize}px;
    border-radius: 6px;
    border: 1px solid rgba(255, 255, 255, 0.05);
    background: transparent;
    color: ${({ theme }) => theme.colors.textPrimary};
    font-size: 20px;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: ${({ theme }) => theme.transitions.normal};

    &:hover {
        border: 1px solid rgba(255, 255, 255, 0.8);
        box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.04);
    }
`;
