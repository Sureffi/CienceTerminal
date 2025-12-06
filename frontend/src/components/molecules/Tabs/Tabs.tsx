import styled from 'styled-components';
import { MOBILE_BREAKPOINT, PAGE_DIMENSIONS } from '@/pages/constants';

interface Tab {
    id: string;
    label: string;
}

interface TabsProps {
    tabs: Tab[];
    activeTab: string;
    onTabChange: (tabId: string) => void;
    className?: string;
}

/**
 * Tabs Component
 *
 * Generic tab navigation component used for filtering/switching views
 */
export const Tabs = ({ tabs, activeTab, onTabChange, className }: TabsProps) => {
    return (
        <TabContainer className={className}>
            {tabs.map((tab) => (
                <Tab
                    key={tab.id}
                    $isActive={activeTab === tab.id}
                    onClick={() => onTabChange(tab.id)}
                >
                    {tab.label}
                </Tab>
            ))}
        </TabContainer>
    );
};

/**
 * Styled Components
 *
 * Responsive Design Pattern (following Header and TokenTable):
 * - Uses constants from @/pages/constants for maintainability
 * - Mobile breakpoint (768px) for consistent responsive behavior
 * - Touch optimization for better mobile UX
 */
const TabContainer = styled.div`
    display: flex;
    gap: ${PAGE_DIMENSIONS.tabs.container.gapDesktop};
    align-items: center;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        gap: ${PAGE_DIMENSIONS.tabs.container.gapMobile};
    }
`;

const Tab = styled.button<{ $isActive: boolean }>`
    padding: ${PAGE_DIMENSIONS.tabs.button.paddingDesktop};
    gap: 10px;
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: ${PAGE_DIMENSIONS.tabs.button.fontSizeDesktop};
    font-weight: ${PAGE_DIMENSIONS.tabs.button.fontWeight};
    text-transform: uppercase;
    border: none;
    background: ${({ $isActive }) =>
        $isActive ? 'rgba(17, 255, 0, 0.1)' : 'rgba(255, 255, 255, 0.1)'};
    color: ${({ $isActive }) =>
        $isActive ? " #11FF00 " : 'rgba(255, 255, 255, 0.6)'};
    border-radius: ${PAGE_DIMENSIONS.tabs.button.borderRadius};
    cursor: pointer;
    transition: all ${({ theme }) => theme.transitions.fast};

    /* Touch optimization - prevents double-tap zoom delay on mobile */
    touch-action: manipulation;

    /* Hover effects only on devices with hover capability */
    @media (hover: hover) {
        &:hover {
            background: ${({ $isActive }) =>
                $isActive ? 'rgba(17, 255, 0, 0.15)' : 'rgba(255, 255, 255, 0.15)'};
        }
    }

    /* Active state for touch feedback */
    &:active {
        transform: scale(0.98);
        background: ${({ $isActive }) =>
            $isActive ? 'rgba(17, 255, 0, 0.2)' : 'rgba(255, 255, 255, 0.2)'};
    }

    /* Mobile responsiveness */
    @media (max-width: ${MOBILE_BREAKPOINT}) {
        padding: ${PAGE_DIMENSIONS.tabs.button.paddingMobile};
        font-size: ${PAGE_DIMENSIONS.tabs.button.fontSizeMobile};
    }
`;
