/**
 * Shared constants for FilterPanel component
 *
 * These constants ensure consistency and maintainability for responsive design.
 * Following the same pattern as Header/constants.ts and TokenTable/constants.ts.
 */

/**
 * Responsive breakpoint for mobile devices
 * Matches Header and TokenTable breakpoint for consistency across the app
 */
export const MOBILE_BREAKPOINT = '768px';

/**
 * Component dimensions for consistent sizing across desktop and mobile
 */
export const DIMENSIONS = {
    header: {
        paddingDesktop: '12px 16px',
        paddingMobile: '10px 12px',
    },
    content: {
        paddingDesktop: '16px',
        paddingMobile: '12px 12px',
    },
    input: {
        paddingDesktop: '8px 12px',
        paddingMobile: '8px 10px', // Slightly larger for better touch targets
        minHeightMobile: '32px', // Minimum touch target size for accessibility
    },
    spacing: {
        headerLeftGapDesktop: '8px',
        headerLeftGapMobile: '6px',
        headerRightGapDesktop: '8px',
        headerRightGapMobile: '6px',
        rangeInputsGapDesktop: '8px',
        rangeInputsGapMobile: '6px', // Reduced gap when stacked vertically
        filterGroupGapDesktop: '6px',
        filterGroupGapMobile: '5px',
        filterRowMarginDesktop: '12px',
        filterRowMarginMobile: '10px',
        sectionMarginDesktop: '16px',
        sectionMarginMobile: '12px',
    },
} as const;

/**
 * Typography for consistent font sizes across desktop and mobile
 */
export const TYPOGRAPHY = {
    filterIcon: {
        sizeDesktop: '18px',
        sizeMobile: '16px',
    },
    title: {
        sizeDesktop: '14px',
        sizeMobile: '13px',
    },
    label: {
        sizeDesktop: '12px',
        sizeMobile: '11px',
    },
    hint: {
        sizeDesktop: '11px',
        sizeMobile: '10px',
    },
    input: {
        sizeDesktop: '13px',
        sizeMobile: '12px',
    },
    rangeSeparator: {
        sizeDesktop: '14px',
        sizeMobile: '14px',
    },
    sectionTitle: {
        sizeDesktop: '12px',
        sizeMobile: '11px',
    },
    button: {
        sizeDesktop: '12px',
        sizeMobile: '12px',
    },
} as const;

/**
 * Z-index for header positioning
 * Uses a value to stay above scrollable content
 */
export const Z_INDEX = {
    header: 10,
} as const;
