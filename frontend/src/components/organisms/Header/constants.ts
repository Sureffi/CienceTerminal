/**
 * Shared constants for Header component
 *
 * These constants ensure consistency and maintainability for responsive design.
 * Following the same pattern as TokenTable/constants.ts for consistency.
 */

/**
 * Responsive breakpoint for mobile devices
 * Matches TokenTable breakpoint for consistency across the app
 */
export const MOBILE_BREAKPOINT = '768px';

/**
 * Component dimensions for consistent sizing across desktop and mobile
 */
export const DIMENSIONS = {
    header: {
        paddingDesktop: '20px',
        paddingMobile: '12px',
    },
    logo: {
        heightDesktop: 32,
        heightMobile: 24,
    },
    buttonIcon: {
        heightDesktop: 14,
        heightMobile: 12,
        widthDesktop: 20,
        widthMobile: 16,
    },
    spacing: {
        leftSectionGapDesktop: '24px', // theme.spacing.lg
        leftSectionGapMobile: '16px',
        navButtonsGapDesktop: '12px', // theme.spacing.sm
        navButtonsGapMobile: '8px',
        rightSectionGapDesktop: '12px', // theme.spacing.sm
        rightSectionGapMobile: '8px',
    },
} as const;

/**
 * Z-index for header positioning
 * Uses a high value to stay above most content while sticky
 */
export const Z_INDEX = {
    header: 100,
} as const;
