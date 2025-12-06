import { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import styled from 'styled-components';
import { TokenTable } from '@/components/organisms/TokenTable';
import { TokenDetailsDrawer } from '@/components/organisms/TokenDetailsDrawer';
import { Tabs } from '@/components/molecules/Tabs';
import { useCaMentionAlerts } from '@/providers/CaMentionAlertProvider';
import { transformCaMentionAlertsToTokens } from '@/utils/caMentionAlertTransformer';
import { useMediaQuery } from '@/hooks/useMediaQuery';
import type { Token, TokenFilterTab } from '@/types/token';
import { MOBILE_BREAKPOINT, PAGE_DIMENSIONS, LAYOUT_HEIGHTS, Z_INDEX } from './constants';

const tabs = [
    { id: 'TRENDING', label: 'TRENDING' },
];

export const ScreenerPage = () => {
    const [activeTab, setActiveTab] = useState<TokenFilterTab>('TRENDING');
    const [selectedToken, setSelectedToken] = useState<Token | null>(null);
    const [isDrawerOpen, setIsDrawerOpen] = useState(false);
    const navigate = useNavigate();
    const isMobile = useMediaQuery(`(max-width: ${MOBILE_BREAKPOINT})`);

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
        if (isMobile) {
            // On mobile: Open drawer instead of navigating
            setSelectedToken(token);
            setIsDrawerOpen(true);
        } else {
            // On desktop: Navigate to search page
            if (token.contractAddress) {
                navigate(`/search/${token.contractAddress}`);
            }
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

/**
 * Styled Components
 *
 * Responsive Design Pattern (following Header and TokenTable):
 * - Uses constants from ./constants for maintainability
 * - Mobile breakpoint (768px) for consistent responsive behavior
 * - Responsive layout heights for proper absolute positioning
 */
const Header = styled.div`
    display: flex;
    align-items: center;
    width: 100%;
    background: ${({ theme }) => theme.colors.background};
    padding-left: ${PAGE_DIMENSIONS.screenerHeader.paddingLeftDesktop};
    padding-top: ${PAGE_DIMENSIONS.screenerHeader.paddingTopDesktop};
    z-index: ${Z_INDEX.pageHeader};

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        padding-left: ${PAGE_DIMENSIONS.screenerHeader.paddingLeftMobile};
        padding-top: ${PAGE_DIMENSIONS.screenerHeader.paddingTopMobile};
    }
`;

const PageContainer = styled.div`
    width: 100%;
    max-width: 100%;
    position: absolute;
    top: ${LAYOUT_HEIGHTS.appHeaderDesktop + LAYOUT_HEIGHTS.tabsHeaderDesktop}px;
    left: 0;
    right: 0;
    bottom: 0;
    justify-content: center;
    overflow: auto;
    background: ${({ theme }) => theme.colors.background};

    /* Touch optimization for smooth scrolling */
    -webkit-overflow-scrolling: touch;
    overscroll-behavior: contain;
    scroll-behavior: smooth;
    scrollbar-gutter: stable;

    /* Mobile optimizations */
    @media (max-width: ${MOBILE_BREAKPOINT}) {
        /* Responsive top positioning for mobile header heights */
        top: ${LAYOUT_HEIGHTS.appHeaderMobile + LAYOUT_HEIGHTS.tabsHeaderMobile}px;

        /* Add safe area insets for notched devices */
        padding-bottom: env(safe-area-inset-bottom);
    }

    /* Fade-to-black gradient at bottom */
    &::after {
        content: '';
        position: sticky;
        bottom: 0;
        left: 0;
        right: 0;
        height: 100px;
        margin-top: -100px;
        background: linear-gradient(to bottom, transparent, black);
        pointer-events: none;
        z-index: ${Z_INDEX.gradientOverlay};
    }
`;
