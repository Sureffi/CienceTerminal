import { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import styled from 'styled-components';
import { TokenTable } from '@/components/organisms/TokenTable';
import { TokenDetailsDrawer } from '@/components/organisms/TokenDetailsDrawer';
import { Tabs } from '@/components/molecules/Tabs';
import { useCaMentionAlerts } from '@/providers/CaMentionAlertProvider';
import { transformCaMentionAlertsToTokens } from '@/utils/caMentionAlertTransformer';
import type { Token, TokenFilterTab } from '@/types/token';

// Layout height constants for consistent layout calculations
const LAYOUT_HEIGHTS = {
    appHeader: 82,
    tabsHeader: 66,
    edgeCoverTop: 140, // appHeader + tabsHeader - adjustment
    edgeCoverOffset: 148,
} as const;

const tabs = [
    { id: 'TRENDING', label: 'TRENDING' },
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
            {/* <LeftEdgeCover /> */}
            {/* <RightEdgeCover /> */}
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
    background: ${({ theme }) => theme.colors.background};
    padding: ${({ theme }) => theme.spacing.lg};
    z-index: 100;
`;

const PageContainer = styled.div`
    width: 100%;
    max-width: 100%;
    height: calc(100vh - ${LAYOUT_HEIGHTS.appHeader}px - ${LAYOUT_HEIGHTS.tabsHeader}px);
    justify-content: center;
    overflow: auto;
    position: fixed;
    background: ${({ theme }) => theme.colors.background};

    /* Touch optimization for smooth scrolling */
    -webkit-overflow-scrolling: touch;
    overscroll-behavior: contain;
    touch-action: pan-y;

    /* Mobile optimizations */
    @media (max-width: 768px) {
        /* Use dvh (dynamic viewport height) for better mobile browser support */
        height: calc(100dvh - ${LAYOUT_HEIGHTS.appHeader}px - ${LAYOUT_HEIGHTS.tabsHeader}px);
        /* Add safe area insets for notched devices */
        padding-bottom: env(safe-area-inset-bottom);
    }

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

        @media (max-width: 768px) {
            bottom: env(safe-area-inset-bottom);
        }
    }
`;

const LeftEdgeCover = styled.div`
    position: fixed;
    left: 0;
    top: ${LAYOUT_HEIGHTS.edgeCoverTop}px;
    width: 15px;
    height: calc(100vh - ${LAYOUT_HEIGHTS.edgeCoverOffset}px);
    background: ${({ theme }) => theme.colors.background};
    z-index: 35;
    pointer-events: none;
`;

const RightEdgeCover = styled.div`
    position: fixed;
    right: 0;
    top: ${LAYOUT_HEIGHTS.edgeCoverTop}px;
    width: 15px;
    height: calc(100vh - ${LAYOUT_HEIGHTS.edgeCoverOffset}px);
    background: ${({ theme }) => theme.colors.background};
    z-index: 35;
    pointer-events: none;
`;
