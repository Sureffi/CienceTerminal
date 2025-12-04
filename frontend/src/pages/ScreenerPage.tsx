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
            <Header>
                <Tabs
                    tabs={tabs}
                    activeTab={activeTab}
                    onTabChange={(tabId) => setActiveTab(tabId as TokenFilterTab)}
                />
            </Header>
            <LeftEdgeCover />
            <RightEdgeCover />
            <PageContainer>
                <TokenTable tokens={tokens} onAddToken={handleAddToken} onRowClick={handleRowClick} />
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
const Header = styled.div`
    display: flex;
    align-items: center;
    width: 100%;
    background: #000000;
    padding: ${({ theme }) => theme.spacing.lg};
    z-index: 100;
`;

const PageContainer = styled.div`
    width: 100%;
    max-width: 100%;
    height: calc(100vh - 82px - 66px); /* Adjust for app header (82px) and tabs header (66px) */
    // display: flex;
    justify-content: center;
    overflow: auto; /* Allow both vertical and horizontal scrolling at page level */
    position: fixed;
    padding-left: 15px;
    padding-right: 15px;
    background: #000000;


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
        z-index: 50;
    }
`;

const LeftEdgeCover = styled.div`
    position: fixed;
    left: 0;
    top: 140px; /* Below header (82px) and tabs (66px) */
    width: 15px;
    height: calc(100vh - 148px);
    background: #000000;
    z-index: 35; /* Above table content (z-index: 1-10), below sticky column (z-index: 40) */
    pointer-events: none;
`;

const RightEdgeCover = styled.div`
    position: fixed;
    right: 0;
    top: 148px;
    width: 15px;
    height: calc(100vh - 148px);
    background: #000000;
    z-index: 35;
    pointer-events: none;
`;



