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
            <HeaderRow>
                <HeaderCell>TOKEN</HeaderCell>
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
    background: transparent;
    border-radius: ${({ theme }) => theme.borderRadius.md};
`;

const HeaderRow = styled.div`
    display: grid;
    grid-template-columns: 250px 150px 80px 150px 130px 100px 110px 90px 100px;
    align-items: center;
    background: #000000;
    margin-bottom: 0px;
    position: sticky;
    top: 66px;
    z-index: 50;
    padding-top: 0px;
    padding-bottom: 12px;
`;

const HeaderCell = styled.div`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 12px;
    font-weight: ${({ theme }) => theme.typography.fontWeight.bold};
    color: ${({ theme }) => theme.colors.textMuted};
    text-transform: uppercase;
    letter-spacing: 0.5px;
`;

const TableBody = styled.div`
    /* Display full list without internal scrolling */
`;
