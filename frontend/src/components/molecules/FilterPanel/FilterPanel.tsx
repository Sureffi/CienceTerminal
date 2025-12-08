import { useState, useEffect } from 'react';
import styled from 'styled-components';
import { FaFilter } from 'react-icons/fa6';
import { MOBILE_BREAKPOINT, DIMENSIONS, TYPOGRAPHY, Z_INDEX } from './constants';

export interface TokenFilters {
    // Market metrics
    marketCapMin?: number;
    marketCapMax?: number;
    volume24hMin?: number;
    volume24hMax?: number;
    liquidityMin?: number;
    liquidityMax?: number;

    // Holder metrics
    holdersCountMin?: number;
    holdersCountMax?: number;
    top10HoldersPercentMax?: number;
    devHoldPercentMax?: number;

    // Social metrics
    mentions24hMin?: number;
    trendScoreMin?: number;

    // Age
    ageHoursMin?: number;
    ageHoursMax?: number;
}

export interface FilterPanelProps {
    filters: TokenFilters;
    onFiltersChange: (filters: TokenFilters) => void;
    onReset: () => void;
    onApply?: () => void;
}

export const FilterPanel = ({ filters, onFiltersChange, onReset, onApply }: FilterPanelProps) => {
    // Local draft state - filters are only applied when user clicks Apply
    const [draftFilters, setDraftFilters] = useState<TokenFilters>(filters);

    // Sync draft filters with applied filters when they change externally (e.g., reset)
    useEffect(() => {
        setDraftFilters(filters);
    }, [filters]);

    const handleInputChange = (field: keyof TokenFilters, value: string) => {
        const numValue = value === '' ? undefined : parseFloat(value);
        setDraftFilters({
            ...draftFilters,
            [field]: numValue,
        });
    };

    const handleApply = () => {
        onFiltersChange(draftFilters);
        onApply?.();
    };

    const handleReset = () => {
        setDraftFilters({});
        onReset();
    };

    return (
        <Container>
            <Header>
                <HeaderLeft>
                    <FilterIcon>
                        <FaFilter />
                    </FilterIcon>
                    <Title>Filters</Title>
                </HeaderLeft>
                <HeaderRight>
                    <ResetButton onClick={handleReset}>Reset</ResetButton>
                    {onApply && (
                        <ApplyButton onClick={handleApply}>Apply</ApplyButton>
                    )}
                </HeaderRight>
            </Header>

            <Content>
                <Section>
                    <FilterRow>
                        <FilterGroup>
                            <Label>Market Cap</Label>
                            <RangeInputs>
                                <Input
                                    type="number"
                                    placeholder="Min"
                                    value={draftFilters.marketCapMin ?? ''}
                                    onChange={(e) => handleInputChange('marketCapMin', e.target.value)}
                                />
                                <RangeSeparator>–</RangeSeparator>
                                <Input
                                    type="number"
                                    placeholder="Max"
                                    value={draftFilters.marketCapMax ?? ''}
                                    onChange={(e) => handleInputChange('marketCapMax', e.target.value)}
                                />
                            </RangeInputs>
                        </FilterGroup>
                    </FilterRow>

                    <FilterRow>
                        <FilterGroup>
                            <Label>24h Volume</Label>
                            <RangeInputs>
                                <Input
                                    type="number"
                                    placeholder="Min"
                                    value={draftFilters.volume24hMin ?? ''}
                                    onChange={(e) => handleInputChange('volume24hMin', e.target.value)}
                                />
                                <RangeSeparator>–</RangeSeparator>
                                <Input
                                    type="number"
                                    placeholder="Max"
                                    value={draftFilters.volume24hMax ?? ''}
                                    onChange={(e) => handleInputChange('volume24hMax', e.target.value)}
                                />
                            </RangeInputs>
                        </FilterGroup>
                    </FilterRow>

                    <FilterRow>
                        <FilterGroup>
                            <Label>Liquidity</Label>
                            <RangeInputs>
                                <Input
                                    type="number"
                                    placeholder="Min"
                                    value={draftFilters.liquidityMin ?? ''}
                                    onChange={(e) => handleInputChange('liquidityMin', e.target.value)}
                                />
                                <RangeSeparator>–</RangeSeparator>
                                <Input
                                    type="number"
                                    placeholder="Max"
                                    value={draftFilters.liquidityMax ?? ''}
                                    onChange={(e) => handleInputChange('liquidityMax', e.target.value)}
                                />
                            </RangeInputs>
                        </FilterGroup>
                    </FilterRow>
                    <FilterRow>
                        <FilterGroup>
                            <Label>Holder Count</Label>
                            <RangeInputs>
                                <Input
                                    type="number"
                                    placeholder="Min"
                                    value={draftFilters.holdersCountMin ?? ''}
                                    onChange={(e) => handleInputChange('holdersCountMin', e.target.value)}
                                />
                                <RangeSeparator>–</RangeSeparator>
                                <Input
                                    type="number"
                                    placeholder="Max"
                                    value={draftFilters.holdersCountMax ?? ''}
                                    onChange={(e) => handleInputChange('holdersCountMax', e.target.value)}
                                />
                            </RangeInputs>
                        </FilterGroup>
                    </FilterRow>

                    <FilterRow>
                        <FilterGroup>
                            <Label>
                                Age
                                <Hint>(hours)</Hint>
                            </Label>
                            <RangeInputs>
                                <Input
                                    type="number"
                                    placeholder="Min"
                                    value={draftFilters.ageHoursMin ?? ''}
                                    onChange={(e) => handleInputChange('ageHoursMin', e.target.value)}
                                />
                                <RangeSeparator>–</RangeSeparator>
                                <Input
                                    type="number"
                                    placeholder="Max"
                                    value={draftFilters.ageHoursMax ?? ''}
                                    onChange={(e) => handleInputChange('ageHoursMax', e.target.value)}
                                />
                            </RangeInputs>
                        </FilterGroup>
                    </FilterRow>
                    <FilterRow>
                        <FilterGroup>
                            <Label>24h Mentions</Label>
                            <Input
                                type="number"
                                placeholder="Min mentions"
                                value={draftFilters.mentions24hMin ?? ''}
                                onChange={(e) => handleInputChange('mentions24hMin', e.target.value)}
                            />
                        </FilterGroup>
                    </FilterRow>

                </Section>
            </Content>
        </Container>
    );
};

// Styled Components

const Container = styled.div`
    background: ${({ theme }) => theme.colors.bgDark};
    border: 1px solid ${({ theme }) => theme.colors.borderDefault};
    border-radius: 6px;;
    overflow: hidden;
    display: flex;
    flex-direction: column;
    max-height: 100%;
    width: 100%;
    box-sizing: border-box;
`;

const Header = styled.div`
    position: sticky;
    top: 0;
    z-index: ${Z_INDEX.header};
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: ${DIMENSIONS.header.paddingDesktop};
    background: ${({ theme }) => theme.colors.bgDark};
    border-bottom: 1px solid ${({ theme }) => theme.colors.borderDefault};

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        padding: ${DIMENSIONS.header.paddingMobile};
    }
`;

const HeaderLeft = styled.div`
    display: flex;
    align-items: center;
    gap: ${DIMENSIONS.spacing.headerLeftGapDesktop};

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        gap: ${DIMENSIONS.spacing.headerLeftGapMobile};
    }
`;

const HeaderRight = styled.div`
    display: flex;
    align-items: center;
    gap: ${DIMENSIONS.spacing.headerRightGapDesktop};

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        gap: ${DIMENSIONS.spacing.headerRightGapMobile};
    }
`;

const FilterIcon = styled.span`
    font-size: ${TYPOGRAPHY.filterIcon.sizeDesktop};
    color: #11FF00;
    display: flex;
    align-items: center;
    justify-content: center;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        font-size: ${TYPOGRAPHY.filterIcon.sizeMobile};
    }
`;

const Title = styled.h3`
    margin: 0;
    font-size: ${TYPOGRAPHY.title.sizeDesktop};
    font-weight: ${({ theme }) => theme.typography.fontWeight.semibold};
    color: ${({ theme }) => theme.colors.textPrimary};
    letter-spacing: 0.5px;
    text-transform: uppercase;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        font-size: ${TYPOGRAPHY.title.sizeMobile};
    }
`;

const ResetButton = styled.button`
    padding: 4px 12px;
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    font-weight: 600;
    text-transform: uppercase;
    border: 2px solid ${({ theme }) => theme.colors.borderGhost};
    background: transparent;
    color: ${({ theme }) => theme.colors.textPrimary};
    opacity: 0.4;
    border-radius: 4px;
    cursor: pointer;
    transition: all ${({ theme }) => theme.transitions.fast};
    touch-action: manipulation;

    /* Hover effects only on devices with hover capability */
    @media (hover: hover) {
        &:hover {
            opacity: 0.6;
            border-color: rgba(255, 255, 255, 0.8);
        }
    }

    /* Active state for touch feedback */
    &:active {
        transform: scale(0.98);
        opacity: 0.8;
    }
`;

const ApplyButton = styled.button`
    padding: 4px 12px;
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: 12px;
    font-weight: 600;
    text-transform: uppercase;
    border: none;
    background: rgba(17, 255, 0, 0.1);
    color: #11FF00;
    border-radius: 4px;
    cursor: pointer;
    transition: all ${({ theme }) => theme.transitions.fast};
    touch-action: manipulation;

    /* Hover effects only on devices with hover capability */
    @media (hover: hover) {
        &:hover {
            background: rgba(17, 255, 0, 0.15);
        }
    }

    /* Active state for touch feedback */
    &:active {
        transform: scale(0.98);
        background: rgba(17, 255, 0, 0.2);
    }
`;

const Content = styled.div`
    padding: ${DIMENSIONS.content.paddingDesktop};
    overflow-y: auto;
    overflow-x: hidden;
    flex: 1;
    width: 100%;
    box-sizing: border-box;
    -webkit-overflow-scrolling: touch;
    overscroll-behavior: contain;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        padding: ${DIMENSIONS.content.paddingMobile};
        transform: translateZ(0); /* GPU acceleration for smooth scrolling on mobile */
    }
`;

const Section = styled.div`
    margin-bottom: ${DIMENSIONS.spacing.sectionMarginDesktop};

    &:last-child {
        margin-bottom: 0;
    }

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        margin-bottom: ${DIMENSIONS.spacing.sectionMarginMobile};
    }
`;

const FilterRow = styled.div`
    margin-bottom: ${DIMENSIONS.spacing.filterRowMarginDesktop};

    &:last-child {
        margin-bottom: 0;
    }

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        margin-bottom: ${DIMENSIONS.spacing.filterRowMarginMobile};
    }
`;

const FilterGroup = styled.div`
    display: flex;
    flex-direction: column;
    gap: ${DIMENSIONS.spacing.filterGroupGapDesktop};
    width: 100%;
    box-sizing: border-box;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        gap: ${DIMENSIONS.spacing.filterGroupGapMobile};
    }
`;

const Label = styled.label`
    display: flex;
    align-items: center;
    gap: 4px;
    font-size: ${TYPOGRAPHY.label.sizeDesktop};
    font-weight: ${({ theme }) => theme.typography.fontWeight.medium};
    color: ${({ theme }) => theme.colors.textPrimary};

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        font-size: ${TYPOGRAPHY.label.sizeMobile};
    }
`;

const Hint = styled.span`
    font-size: ${TYPOGRAPHY.hint.sizeDesktop};
    color: ${({ theme }) => theme.colors.textMuted};
    font-weight: ${({ theme }) => theme.typography.fontWeight.normal};

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        font-size: ${TYPOGRAPHY.hint.sizeMobile};
    }
`;

const RangeInputs = styled.div`
    display: flex;
    align-items: center;
    gap: ${DIMENSIONS.spacing.rangeInputsGapDesktop};
    width: 100%;
    box-sizing: border-box;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        flex-direction: column;
        align-items: stretch;
        gap: ${DIMENSIONS.spacing.rangeInputsGapMobile};
    }
`;

const RangeSeparator = styled.span`
    color: ${({ theme }) => theme.colors.textMuted};
    font-size: ${TYPOGRAPHY.rangeSeparator.sizeDesktop};
    flex-shrink: 0;

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        display: none; /* Hide separator when inputs are stacked vertically */
    }
`;

const Input = styled.input`
    flex: 1;
    min-width: 0;
    padding: ${DIMENSIONS.input.paddingDesktop};
    font-size: ${TYPOGRAPHY.input.sizeDesktop};
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    color: ${({ theme }) => theme.colors.textPrimary};
    background: ${({ theme }) => theme.colors.bgDark};
    border: 1px solid ${({ theme }) => theme.colors.borderDefault};
    border-radius: ${({ theme }) => theme.borderRadius.sm};
    transition: all ${({ theme }) => theme.transitions.fast};
    box-sizing: border-box;
    touch-action: manipulation; /* Prevent double-tap zoom on mobile */

    &::placeholder {
        color: ${({ theme }) => theme.colors.textMuted};
    }

    &:focus {
        outline: none;
        border-color: ${({ theme }) => theme.colors.accentGreen};
        box-shadow: 0 0 0 2px rgba(19, 222, 46, 0.1);
    }

    /* Remove spinner arrows for number inputs */
    &::-webkit-outer-spin-button,
    &::-webkit-inner-spin-button {
        -webkit-appearance: none;
        margin: 0;
    }

    &[type='number'] {
        -moz-appearance: textfield;
    }

    @media (max-width: ${MOBILE_BREAKPOINT}) {
        padding: ${DIMENSIONS.input.paddingMobile};
        font-size: ${TYPOGRAPHY.input.sizeMobile};
        min-height: ${DIMENSIONS.input.minHeightMobile}; /* Better touch target size */
    }
`;
