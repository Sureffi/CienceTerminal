import styled from 'styled-components';
import type { Token } from '@/types/token';
import { TokenRow } from '@/components/molecules/TokenRow';

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
                    {/* <HeaderCell>CHART</HeaderCell> */}
                    <HeaderCell>AGE</HeaderCell>
                    <HeaderCell>PRICE CHANGE</HeaderCell>
                    <HeaderCell>MARKET CAP</HeaderCell>
                    <HeaderCell>VOLUME</HeaderCell>
                    <HeaderCell>LIQUIDITY</HeaderCell>
                    <HeaderCell>HOLDERS</HeaderCell>
                    <HeaderCell>TOP 10 HLDRS</HeaderCell>
                    {/* <HeaderCell>DEV</HeaderCell> */}
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
    background: #000000;
`;

const HeaderContainer = styled.div`
    display: flex;
    align-items: center;
    position: sticky;
    top: 0;
    padding-bottom: 10px;
    z-index: 50;
`;

const HeaderRow = styled.div`
    display: grid;
    align-items: center;
    grid-template-columns: minmax(200px, 2fr) minmax(120px, 1.5fr) minmax(80px, 0.8fr) minmax(120px, 1.2fr) minmax(110px, 1.2fr) minmax(100px, 1fr) minmax(100px, 1fr) minmax(90px, 1fr) minmax(100px, 1fr);
    align-items: center;
    flex: 1;
    min-width: 1120px;
`;

const HeaderSpacer = styled.div`
    min-width: 50px;
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
    background: #000000;

    ${({ $isSticky }) =>
        $isSticky &&
        `
        position: sticky;
        left: 0;
        z-index: 60;
        background: #000000;
        will-change: transform;
        transform: translateZ(0);
        backface-visibility: hidden;
    `}
`;

const TableBody = styled.div`
    background: #000000;
    /* Display full list without internal scrolling */
`;
