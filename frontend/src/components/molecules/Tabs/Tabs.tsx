import styled from 'styled-components';

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

// Styled Components
const TabContainer = styled.div`
    display: flex;
    gap: 5px;
    align-items: center;
`;

const Tab = styled.button<{ $isActive: boolean }>`
    padding: 2px 5px;
    gap: 10px;
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    font-weight: 600;
    text-transform: uppercase;
    border: none;
    background: ${({ $isActive }) =>
        $isActive ? 'rgba(17, 255, 0, 0.1)' : 'rgba(255, 255, 255, 0.1)'};
    color: ${({ $isActive }) =>
        $isActive ? " #11FF00 " : 'rgba(255, 255, 255, 0.6)'};
    border-radius: 4px;
    cursor: pointer;
    transition: all ${({ theme }) => theme.transitions.fast};
`;
