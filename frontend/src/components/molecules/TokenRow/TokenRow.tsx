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
                        <StyledTokenIdentifier symbol={token.symbol} blockchain={token.blockchain} size="medium" />
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

    /* Mobile: scale down for better fit*/
    @media (max-width: 768px) {
        margin-bottom: 3px;
        gap: 3px;
    }
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
    background: ${({ theme }) => theme.colors.bgTransparent};
    touch-action: manipulation;

    /* Only apply hover effects on devices that support hover */
    @media (hover: hover) {
        &:hover {
            box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.04);
        }
    }

    /* Touch feedback for mobile */
    &:active {
        box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.08);
    }

    /* Mobile: scale down for better fit*/
    @media (max-width: 768px) {
        height: 32px;
        min-width: 900px; /* Smaller but still requires horizontal scroll */
        grid-template-columns: minmax(120px, 1.0fr) minmax(90px, 1.2fr) minmax(60px, 0.7fr) minmax(90px, 1fr) minmax(85px, 1fr) minmax(75px, 0.8fr) minmax(75px, 0.8fr) minmax(70px, 0.8fr) minmax(80px, 0.9fr);
    }
`;

const Cell = styled.div<{ $isSticky?: boolean }>`
    display: flex;
    align-items: center;
    border-top: 1px solid rgba(255, 255, 255, 0.05);
    border-bottom: 1px solid rgba(255, 255, 255, 0.05);
    height: 100%;
    transition: border-color ${({ theme }) => theme.transitions.normal};
    background: #000000;

    /* Only apply hover border changes on hover-capable devices */
    @media (hover: hover) {
        ${Row}:hover & {
            border-top-color: rgba(255, 255, 255, 0.8);
            border-bottom-color: rgba(255, 255, 255, 0.8);
        }
    }

    &:last-child {
        border-right: 1px solid rgba(255, 255, 255, 0.05);
        border-top-right-radius: 6px;
        border-bottom-right-radius: 6px;

        @media (hover: hover) {
            ${Row}:hover & {
                border-right-color: rgba(255, 255, 255, 0.8);
            }
        }
    }

    ${({ $isSticky }) =>
        $isSticky &&
        `
        ${stickyColumnStyles(Z_INDEX.stickyColumn)}
        border-left: 1px solid rgba(255, 255, 255, 0.05);
        border-top-left-radius: 6px;
        border-bottom-left-radius: 6px;
        box-shadow: 4px 0 12px -4px rgba(0, 0, 0, 0.5);

        /* Mobile: scale down for better fit*/
        @media (max-width: 768px) {
            // border-right: 1px solid rgba(255, 255, 255, 0.1);
            background: #131B1475;
            backdrop-filter: blur(10px);
        }

    `}

    @media (hover: hover) {
        ${Row}:hover & {
            ${({ $isSticky }) =>
        $isSticky &&
        `
                border-left-color: rgba(255, 255, 255, 0.8);
            `}
        }
    }
`;

const TokenInfo = styled.div`
    background: ${({ theme }) => theme.colors.bgTransparent},
    align-items: center;
    display: flex;
    position: relative;
    gap: 16px;

    /* Mobile: scale down for better fit*/
    @media (max-width: 768px) {
        gap: 5px;
    }
`;

const TokenIcon = styled.img`
    width: ${DIMENSIONS.tokenIconSize}px;
    height: ${DIMENSIONS.tokenIconSize}px;
    border-radius: 3px;
    margin: 5px;

    @media (max-width: 768px) {
        width: 24px;
        height: 24px;
        margin: 3px;
        border-radius: 2px;
    }
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

    @media (max-width: 768px) {
        width: 20px;
        height: 20px;
        border-width: 1.5px;
        margin-left: -10px;

        &:first-child {
            margin-left: 0;
        }

        /* Show max 2 avatars on mobile to save space */
        &:nth-child(n+3) {
            display: none;
        }
    }
`;

const MentionCount = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    color: ${({ theme }) => theme.colors.textPrimary};

    @media (max-width: 768px) {
        font-size: 11px;
    }
`;

const MetricText = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    color: ${({ theme }) => theme.colors.textPrimary};

    @media (max-width: 768px) {
        font-size: 11px;
    }
`;

const PriceChangeText = styled.span<{ $isPositive: boolean }>`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    color: ${({ theme, $isPositive }) => ($isPositive ? theme.colors.pricePositive : theme.colors.priceNegative)};

    @media (max-width: 768px) {
        font-size: 11px;
    }
`;

const StyledTokenIdentifier = styled(TokenIdentifier)`
    /* Custom theming for TokenRow context */
    /* You can override colors, sizes, spacing, etc. */

    /* Example: Custom color for token name */
    span:first-child {
        color: ${({ theme }) => theme.colors.textPrimary};
    }

    /* Example: Custom opacity for blockchain */
    span:last-child {
        opacity: 0.4;
    }

    /* Mobile: Use small size variant */
    @media (max-width: 768px) {
        font-size: 11px;
        span:first-child {
            color: ${({ theme }) => theme.colors.textPrimary};
            font-weight: ${({ theme }) => theme.typography.fontWeight.medium};
        }
    }
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

    /* Only apply hover on hover-capable devices */
    @media (hover: hover) {
        &:hover {
            border: 1px solid rgba(255, 255, 255, 0.8);
            box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.04);
        }
    }

    /* Touch feedback */
    &:active {
        border: 1px solid rgba(255, 255, 255, 0.8);
        box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.08);
    }

    @media (max-width: 768px) {
        height: 32px;
        min-width: 32px;
        font-size: 18px;
        border-radius: 5px;
    }
`;
