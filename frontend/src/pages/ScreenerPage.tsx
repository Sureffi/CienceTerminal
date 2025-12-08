import { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import styled from 'styled-components';
import { TokenTable } from '@/components/organisms/TokenTable';
import { TokenDetailsDrawer } from '@/components/organisms/TokenDetailsDrawer';
import { Tabs } from '@/components/molecules/Tabs';
import { FilterPanel, type TokenFilters } from '@/components/molecules/FilterPanel';
import { Button } from '@/components/atoms/Button';
import { useCaMentionAlerts } from '@/providers/CaMentionAlertProvider';
import { transformCaMentionAlertsToTokens } from '@/utils/caMentionAlertTransformer';
import { applyTokenFilters } from '@/utils/tokenFilters';
import { useMediaQuery } from '@/hooks/useMediaQuery';
import type { Token, TokenFilterTab } from '@/types/token';
import { MOBILE_BREAKPOINT, PAGE_DIMENSIONS, LAYOUT_HEIGHTS, Z_INDEX } from './constants';
import { FaFilter } from "react-icons/fa6";

const tabs = [
    { id: 'TRENDING', label: 'TRENDING' },
];

const initialFilters: TokenFilters = {};

export const ScreenerPage = () => {
    const [activeTab, setActiveTab] = useState<TokenFilterTab>('TRENDING');
    const [selectedToken, setSelectedToken] = useState<Token | null>(null);
    const [isDrawerOpen, setIsDrawerOpen] = useState(false);
    const [isFilterPanelOpen, setIsFilterPanelOpen] = useState(false);
    const [filters, setFilters] = useState<TokenFilters>(initialFilters);
    const navigate = useNavigate();
    const isMobile = useMediaQuery(`(max-width: ${MOBILE_BREAKPOINT})`);

    // Get real-time CA mention alerts from backend
    const { alerts } = useCaMentionAlerts();

    // Transform CA mention alerts to Token format
    const allTokens = useMemo(
        () => transformCaMentionAlertsToTokens(alerts),
        [alerts]
    );

    // Apply filters first, then sort based on active tab
    const tokens = useMemo(() => {
        // Apply filters
        const filteredTokens = applyTokenFilters(allTokens, filters);

        // Sort based on active tab
        switch (activeTab) {
            case 'TRENDING':
                // Sort by trend score (EMA-based momentum indicator)
                return [...filteredTokens].sort((a, b) => {
                    const aTrend = a.trendScore ?? 0;
                    const bTrend = b.trendScore ?? 0;
                    return bTrend - aTrend;
                });
            case 'TOP':
                // Sort by market cap
                return [...filteredTokens].sort((a, b) => b.marketCap - a.marketCap);
            case 'NEW':
                // Sort by age (newest first)
                return [...filteredTokens].sort((a, b) => {
                    const aTime = a.createdAt?.getTime() || 0;
                    const bTime = b.createdAt?.getTime() || 0;
                    return bTime - aTime;
                });
            default:
                return filteredTokens;
        }
    }, [allTokens, activeTab, filters]);

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

    const handleFiltersChange = (newFilters: TokenFilters) => {
        setFilters(newFilters);
    };

    const handleResetFilters = () => {
        setFilters(initialFilters);
    };

    const handleToggleFilterPanel = () => {
        setIsFilterPanelOpen(!isFilterPanelOpen);
    };

    const handleCloseFilterPanel = () => {
        setIsFilterPanelOpen(false);
    };

    return (
        <>
            <Header>
                <HeaderContent>
                    <Tabs
                        tabs={tabs}
                        activeTab={activeTab}
                        onTabChange={(tabId) => setActiveTab(tabId as TokenFilterTab)}
                    />
                    <FilterButtonWrapper>
                        <Button
                            size="lg"
                            onClick={handleToggleFilterPanel}
                            leftIcon={<FilterIcon />}
                        >
                            {!isMobile && "Filters"}
                        </Button>
                    </FilterButtonWrapper>
                </HeaderContent>
            </Header>
            <PageContainer>
                <TokenTable tokens={tokens} onAddToken={handleAddToken} onRowClick={handleRowClick} />
            </PageContainer>
            <TokenDetailsDrawer
                isOpen={isDrawerOpen}
                onClose={handleCloseDrawer}
                token={selectedToken}
            />
            {isFilterPanelOpen && (
                <FilterOverlay onClick={handleCloseFilterPanel}>
                    <FilterPanelContainer onClick={(e) => e.stopPropagation()}>
                        <FilterPanel
                            filters={filters}
                            onFiltersChange={handleFiltersChange}
                            onReset={handleResetFilters}
                            onApply={handleCloseFilterPanel}
                        />
                    </FilterPanelContainer>
                </FilterOverlay>
            )}
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
    flex-direction: column;
    width: 100%;
    background: ${({ theme }) => theme.colors.background};
    padding-left: ${PAGE_DIMENSIONS.screenerPage.horizontalPaddingDesktop};
    padding-right: ${PAGE_DIMENSIONS.screenerPage.horizontalPaddingDesktop};
    padding-top: ${PAGE_DIMENSIONS.screenerHeader.paddingTopDesktop};
    z-index: ${Z_INDEX.pageHeader};

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        padding-left: ${PAGE_DIMENSIONS.screenerPage.horizontalPaddingMobile};
        padding-right: ${PAGE_DIMENSIONS.screenerPage.horizontalPaddingMobile};
        padding-top: ${PAGE_DIMENSIONS.screenerHeader.paddingTopMobile};
    }
`;

const HeaderContent = styled.div`
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 16px;
    width: 100%;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        gap: 12px;
        padding-left: 8px;
    }
`;

const FilterButtonWrapper = styled.div`
    /* No margin needed on desktop since Header has proper padding */
    position: center;
    justify-content: center;
    
    @media (max-width: ${MOBILE_BREAKPOINT}) {
        /* Add right padding on mobile since page has 0px horizontal padding */
        padding-right: 8px;

        /* Adjust button styling for icon-only display */
        button {
            /* Make button more square and compact for icon-only state */
            min-width: 32px;
            padding: 5px;

            /* Ensure adequate touch target size */
            min-height: 32px;

            /* Remove border on mobile for cleaner look */
            border: none;

            /* Remove all hover effects on mobile */
            &:hover {
                background: ${({ theme }) => theme.colors.bgDark};
                border-color: transparent;
                box-shadow: none;
            }
        }
    }
`;

const FilterOverlay = styled.div`
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.35);
    backdrop-filter: blur(2px);
    display: flex;
    align-items: flex-start;
    justify-content: center;
    padding: 20px;
    z-index: ${Z_INDEX.filterOverlay};
    overflow-y: auto;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        padding: 10px;
        align-items: center;
    }
`;

const FilterPanelContainer = styled.div`
    margin-top: 80px;
    max-width: 500px;
    width: 100%;
    max-height: calc(100vh - 100px);
    overflow-y: auto;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        margin-top: 0;
        max-width: 100%;
        max-height: calc(100vh - 20px);
    }

    /* Custom scrollbar */
    &::-webkit-scrollbar {
        width: 8px;
    }

    &::-webkit-scrollbar-track {
        background: transparent;
    }

    &::-webkit-scrollbar-thumb {
        background: ${({ theme }) => theme.colors.borderDefault};
        border-radius: 4px;

        &:hover {
            background: ${({ theme }) => theme.colors.accentGreen};
        }
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

    padding-left: ${PAGE_DIMENSIONS.screenerPage.horizontalPaddingDesktop};
    padding-right: ${PAGE_DIMENSIONS.screenerPage.horizontalPaddingDesktop};

    /* Touch optimization for smooth scrolling */
    -webkit-overflow-scrolling: touch;
    overscroll-behavior: contain;
    scroll-behavior: smooth;
    scrollbar-gutter: stable;

    /* Mobile optimizations */
    @media (max-width: ${MOBILE_BREAKPOINT}) {
        /* Responsive top positioning for mobile header heights */
        top: ${LAYOUT_HEIGHTS.appHeaderMobile + LAYOUT_HEIGHTS.tabsHeaderMobile}px;

        padding-left: ${PAGE_DIMENSIONS.screenerPage.horizontalPaddingMobile};
        padding-right: ${PAGE_DIMENSIONS.screenerPage.horizontalPaddingMobile};
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

const FilterIcon = styled(FaFilter)`
    color: #11FF00;
`;
