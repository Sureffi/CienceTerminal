import styled from 'styled-components';

interface TokenIdentifierProps {
    symbol: string;
    blockchain: string;
}

/**
 * TokenIdentifier Component
 *
 * Displays token symbol with blockchain network in a consistent format
 * Used across token lists, watchlists, and alerts
 */
export const TokenIdentifier = ({ symbol, blockchain }: TokenIdentifierProps) => {
    return (
        <Container>
            <TokenName>{symbol}</TokenName>
            <Blockchain>  / {blockchain}</Blockchain>
        </Container>
    );
};

// Styled Components
const Container = styled.div`
    display: flex;
`;

const TokenName = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    font-weight: ${({ theme }) => theme.typography.fontWeight.semibold};
    color: ${({ theme }) => theme.colors.textPrimary};
    margin-right: 8px;
`;

const Blockchain = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    font-weight: ${({ theme }) => theme.typography.fontWeight.normal};
    color: ${({ theme }) => theme.colors.textPrimary};
    opacity: 0.4;
    text-transform: uppercase;
`;
