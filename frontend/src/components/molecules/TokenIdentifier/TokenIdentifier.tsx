import styled from 'styled-components';

interface TokenIdentifierProps {
    symbol: string;
    blockchain: string;
    className?: string;
    size?: 'small' | 'medium' | 'large';
}

/**
 * TokenIdentifier Component
 *
 * Displays token symbol with blockchain network in a consistent format
 * Used across token lists, watchlists, and alerts
 *
 * @param className - Optional className for custom styling via styled-components
 * @param size - Optional size variant (small, medium, large)
 */
export const TokenIdentifier = ({
    symbol,
    blockchain,
    className,
    size = 'medium'
}: TokenIdentifierProps) => {
    return (
        <Container className={className} $size={size}>
            <TokenName>{symbol}</TokenName>
            <Blockchain>  / {blockchain}</Blockchain>
        </Container>
    );
};

// Size mappings for font sizes
const sizeMap = {
    small: '10px',
    medium: '12px',
    large: '14px'
};

// Styled Components
const Container = styled.div<{ $size: 'small' | 'medium' | 'large' }>`
    display: flex;
    font-size: ${({ $size }) => sizeMap[$size]};
    align-items: center;
`;

const TokenName = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: inherit;
    font-weight: ${({ theme }) => theme.typography.fontWeight.semibold};
    color: ${({ theme }) => theme.colors.textPrimary};
    margin-right: 8px;
`;

const Blockchain = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: inherit;
    font-weight: ${({ theme }) => theme.typography.fontWeight.normal};
    color: ${({ theme }) => theme.colors.textPrimary};
    opacity: 0.4;
    text-transform: uppercase;

    /* Hide blockchain indicator on mobile to save space */
    @media (max-width: 768px) {
        display: none;
    }
`;
