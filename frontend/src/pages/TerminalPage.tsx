import styled from 'styled-components';
import { TweetCard } from "@/components/organisms";
import { useTwitterAlerts } from "@/providers/TwitterAlertProvider";
import { transformTwitterAlertsToTokens } from "@/utils/twitterAlertTransformer";

export const TerminalPage = () => {
    // Get real-time Twitter alerts from backend
    const { alerts, isConnected, connectionError } = useTwitterAlerts();

    // Transform Twitter alerts to Token format
    const tokens = transformTwitterAlertsToTokens(alerts);

    // Consider showing a connection indicator, loading state, or error message
    // when isConnected is false or connectionError is present

    return (
        <Container>
            <GridContainer>
                {tokens.map(token => (
                    <TweetCard key={token.id} token={token} />
                ))}
            </GridContainer>
        </Container>
    );
};

const Container = styled.div`
    min-height: 100vh;
    background: ${({ theme }) => theme.colors.bgDark};
    padding: 40px 20px;
    justify-content: center;
`;

const GridContainer = styled.div`
    display: grid;
    grid-template-columns: repeat(auto-fill, 360px);
    gap: 20px;
    width: 100%;
    max-width: 1920px;
    margin: 0 auto;
    padding: 0 20px;
    justify-content: center;
`;
