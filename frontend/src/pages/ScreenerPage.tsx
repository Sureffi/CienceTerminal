import { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import styled from 'styled-components';
import { TokenTable } from '@/components/organisms/TokenTable';
import { TokenDetailsDrawer } from '@/components/organisms/TokenDetailsDrawer';
import { Tabs } from '@/components/molecules/Tabs';
import { useCaMentionAlerts } from '@/providers/CaMentionAlertProvider';
import { transformCaMentionAlertsToTokens } from '@/utils/caMentionAlertTransformer';
import type { Token, TokenFilterTab } from '@/types/token';

const tabs = [
    { id: 'TRENDING', label: 'TRENDING' },
    // { id: 'TOP', label: 'TOP' },
    // { id: 'NEW', label: 'NEW' },
];

export const ScreenerPage = () => {
    const [activeTab, setActiveTab] = useState<TokenFilterTab>('TRENDING');
    const [selectedToken, setSelectedToken] = useState<Token | null>(null);
    const [isDrawerOpen, setIsDrawerOpen] = useState(false);
    const navigate = useNavigate();

    // Get real-time CA mention alerts from backend
    const { alerts } = useCaMentionAlerts();

    // Transform CA mention alerts to Token format
    const allTokens = useMemo(
        () => transformCaMentionAlertsToTokens(alerts),
        [alerts]
    );

    // Filter/sort tokens based on active tab
    const tokens = useMemo(() => {
        switch (activeTab) {
            case 'TRENDING':
                // Sort by trend score (EMA-based momentum indicator)
                return [...allTokens].sort((a, b) => {
                    const aTrend = a.trendScore ?? 0;
                    const bTrend = b.trendScore ?? 0;
                    return bTrend - aTrend;
                });
            case 'TOP':
                // Sort by market cap
                return [...allTokens].sort((a, b) => b.marketCap - a.marketCap);
            case 'NEW':
                // Sort by age (newest first)
                return [...allTokens].sort((a, b) => {
                    const aTime = a.createdAt?.getTime() || 0;
                    const bTime = b.createdAt?.getTime() || 0;
                    return bTime - aTime;
                });
            default:
                return allTokens;
        }
    }, [allTokens, activeTab]);

    const handleAddToken = (token: Token) => {
        // Open drawer when + button is clicked
        setSelectedToken(token);
        setIsDrawerOpen(true);
    };

    const handleRowClick = (token: Token) => {
        // Navigate to search page when row is clicked
        if (token.contractAddress) {
            navigate(`/search/${token.contractAddress}`);
        }
    };

    const handleCloseDrawer = () => {
        setIsDrawerOpen(false);
        setSelectedToken(null);
    };

    return (
        <>
            <PageContainer>
                <CenteredContent>
                    <Header>
                        <Tabs
                            tabs={tabs}
                            activeTab={activeTab}
                            onTabChange={(tabId) => setActiveTab(tabId as TokenFilterTab)}
                        />
                    </Header>
                    <TokenTable tokens={tokens} onAddToken={handleAddToken} onRowClick={handleRowClick} />
                </CenteredContent>
            </PageContainer>
            <TokenDetailsDrawer
                isOpen={isDrawerOpen}
                onClose={handleCloseDrawer}
                token={selectedToken}
            />
        </>
    );
};

// Styled Components
const PageContainer = styled.div`
    width: 100%;
    height: calc(100vh - 82px); /* Adjust based on header height */
    display: flex;
    justify-content: center;
    align-items: flex-start;
    overflow-y: auto;
    position: relative;

    /* Fade-to-black gradient at bottom */
    &::after {
        content: '';
        position: fixed;
        bottom: 0;
        left: 0;
        right: 0;
        height: 100px;
        background: linear-gradient(to bottom, transparent, black);
        pointer-events: none;
        z-index: 10;
    }
`;

const CenteredContent = styled.div`
    display: flex;
    flex-direction: column;
    width: fit-content;
    padding: ${({ theme }) => theme.spacing.lg};
    padding-top: 0;
`;

const Header = styled.div`
    display: flex;
    align-items: center;
    position: sticky;
    top: 0;
    z-index: 100;
    background: #000000;
    padding-bottom: ${({ theme }) => theme.spacing.lg};
    padding-top: ${({ theme }) => theme.spacing.lg};
    margin-left: calc(-1 * ${({ theme }) => theme.spacing.lg});
    margin-right: calc(-1 * ${({ theme }) => theme.spacing.lg});
    padding-left: ${({ theme }) => theme.spacing.lg};
    padding-right: ${({ theme }) => theme.spacing.lg};
`;
