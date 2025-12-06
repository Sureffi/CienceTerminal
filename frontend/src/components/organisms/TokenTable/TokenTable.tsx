import styled from 'styled-components';
import type { Token } from '@/types/token';
import { TokenRow } from '@/components/molecules/TokenRow';
import { GRID_COLUMNS, MIN_TABLE_WIDTH, Z_INDEX, DIMENSIONS, stickyColumnStyles } from './constants';

interface TokenTableProps {
    tokens: Token[];
    onAddToken?: (token: Token) => void;
    onRowClick?: (token: Token) => void;
}

/**
 * TokenTable Component
 *
 * Displays the complete token screener table with header and rows
 */
export const TokenTable = ({ tokens, onAddToken, onRowClick }: TokenTableProps) => {
    return (
        <TableContainer>
            {/* Table Header */}
            <HeaderContainer>
                <HeaderRow>
                    <HeaderCell $isSticky>TOKEN</HeaderCell>
                    <HeaderCell>24H MENTIONS</HeaderCell>
                    <HeaderCell>AGE</HeaderCell>
                    <HeaderCell>PRICE CHANGE</HeaderCell>
                    <HeaderCell>MARKET CAP</HeaderCell>
                    <HeaderCell>VOLUME</HeaderCell>
                    <HeaderCell>LIQUIDITY</HeaderCell>
                    <HeaderCell>HOLDERS</HeaderCell>
                    <HeaderCell>TOP 10 HLDRS</HeaderCell>
                </HeaderRow>
                <HeaderSpacer />
            </HeaderContainer>

            {/* Table Body */}
            <TableBody>
                {tokens.map((token) => (
                    <TokenRow key={token.id} token={token} onAdd={onAddToken} onRowClick={onRowClick} />
                ))}
            </TableBody>
        </TableContainer>
    );
};

// Styled Components
const TableContainer = styled.div`
    width: 100%;
    max-width: 100%;
    background: ${({ theme }) => theme.colors.background};
`;

const HeaderContainer = styled.div`
    display: flex;
    align-items: center;
    position: sticky;
    top: 0;
    padding-bottom: 10px;
    z-index: ${Z_INDEX.header};
`;

const HeaderRow = styled.div`
    display: grid;
    align-items: center;
    grid-template-columns: ${GRID_COLUMNS};
    flex: 1;
    min-width: ${MIN_TABLE_WIDTH};

    @media (max-width: 768px) {
        min-width: 900px;
        grid-template-columns: minmax(140px, 1.8fr) minmax(90px, 1.2fr) minmax(60px, 0.7fr) minmax(90px, 1fr) minmax(85px, 1fr) minmax(75px, 0.8fr) minmax(75px, 0.8fr) minmax(70px, 0.8fr) minmax(80px, 0.9fr);
    }
`;

const HeaderSpacer = styled.div`
    min-width: ${DIMENSIONS.headerSpacerWidth}px;

    @media (max-width: 768px) {
        min-width: 40px;
    }
`;

const HeaderCell = styled.div<{ $isSticky?: boolean }>`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 12px;
    font-weight: ${({ theme }) => theme.typography.fontWeight.bold};
    color: ${({ theme }) => theme.colors.textMuted};
    text-transform: uppercase;
    letter-spacing: 0.5px;
    display: flex;
    align-items: center;
    background: ${({ theme }) => theme.colors.background};

    ${({ $isSticky }) =>
        $isSticky &&
        `
        ${stickyColumnStyles(Z_INDEX.stickyHeaderIntersection)}
    `}

    @media (max-width: 768px) {
        font-size: 10px;
        letter-spacing: 0.3px;
    }
`;

const TableBody = styled.div`
    background: ${({ theme }) => theme.colors.background};
`;
