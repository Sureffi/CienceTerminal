import type { Token } from '@/types/token';
import styled from 'styled-components';
import { TokenIdentifier } from '../TokenIdentifier';

interface TokenCardProps {
    token: Token;
}

const formatNumber = (num: number): string => {
    if (num >= 1_000_000) return `${(num / 1_000_000).toFixed(1)}M`;
    if (num >= 1_000) return `${(num / 1_000).toFixed(1)}K`;
    return num.toString();
};

export const TokenCard = ({ token }: TokenCardProps) => {
    return (
        <Container>
            {/* Left Section: Token Info */}
            <LeftSection>
                <TokenIcon src={token.iconUrl} alt={token.symbol} />
                <TokenInfo>
                    <TokenIdentifier symbol={token.symbol} blockchain={token.blockchain} />
                    <MetricsSection>
                        <Metric>
                            <MetricLabel>MCAP</MetricLabel>
                            <MetricValue>{formatNumber(token.marketCap)}</MetricValue>
                        </Metric>
                        <Metric>
                            <MetricLabel>VOL</MetricLabel>
                            <MetricValue>{formatNumber(token.volume24h)}</MetricValue>
                        </Metric>
                    </MetricsSection>

                </TokenInfo>
            </LeftSection>

            {/* Right Section: Chart */}
            <ChartSection>
                <div style={{ background: "white", color: "white" }}></div>
            </ChartSection>
        </Container>
    );
};

// Styled Components
const Container = styled.div`
    display: flex;
    background: rgba(63, 255, 136, 0.04);
    border: 1px solid rgba(0, 255, 42, 0.11);
    border-radius: 4px;
    padding: 8px 10px;
    gap: 34px;
`;

const LeftSection = styled.div`
    display: flex;
    align-items: center;
    gap: 16px;  
`;

const TokenIcon = styled.img`
    width: 34px;
    height: 34px;
    border-radius: 3px;
`;

const TokenInfo = styled.div`
    display: flex;
    flex-direction: column;
    align-items: baseline;
    gap: 6px;
`;

const MetricsSection = styled.div`
    display: flex;
    gap: 16px;
    align-items: center;
`;

const Metric = styled.div`
    display: flex;
    align-items: baseline;
    gap: 6px;
`;

const MetricLabel = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 10px;
    font-weight: 600;
    color: ${({ theme }) => theme.colors.textPrimary};
    opacity: 0.5;
    text-transform: uppercase;
`;

const MetricValue = styled.span`
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: 10px;
    font-weight: ${({ theme }) => theme.typography.fontWeight.semibold};
    color: ${({ theme }) => theme.colors.textPrimary};
`;

const ChartSection = styled.div`
    flex-shrink: 0;
`;
