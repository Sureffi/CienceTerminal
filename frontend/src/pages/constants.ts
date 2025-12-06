/**
 * Shared constants for page-level layouts
 *
 * These constants ensure consistency across pages and maintain responsive design.
 * Following the same pattern as Header/constants.ts and TokenTable/constants.ts.
 */

/**
 * Responsive breakpoint for mobile devices
 * Matches Header and TokenTable breakpoint for consistency
 */
export const MOBILE_BREAKPOINT = '768px';

/**
 * Page-level dimensions for consistent sizing across desktop and mobile
 */
export const PAGE_DIMENSIONS = {
    screenerHeader: {
        paddingLeftDesktop: '5px',
        paddingLeftMobile: '5px',
        paddingTopDesktop: '10px',
        paddingTopMobile: '8px',
    },
    tabs: {
        container: {
            gapDesktop: '5px',
            gapMobile: '3px',
        },
        button: {
            paddingDesktop: '2px 5px',
            paddingMobile: '2px 5px', // Increased for better touch targets
            fontSizeDesktop: '12px',
            fontSizeMobile: '11px',
            fontWeight: 600,
            borderRadius: '4px',
        },
    },
} as const;

/**
 * Layout height constants for absolute positioning calculations
 *
 * These heights must account for the actual rendered heights of Header components.
 * They are calculated based on:
 * - App Header (organisms/Header): padding + logo + border
 * - Tabs Header (pages/ScreenerPage Header): padding + tab height
 */
export const LAYOUT_HEIGHTS = {
    // Main app header with logo and navigation
    // Desktop: 20px (top padding) + ~32px (logo/content) + 20px (bottom padding) + ~10px (buffer) = 82px
    // Mobile: 12px (top padding) + ~24px (logo/content) + 12px (bottom padding) + ~18px (buffer) = 66px
    appHeaderDesktop: 70,
    appHeaderMobile: 62,

    // Tabs section header
    // Desktop: 10px (top padding) + ~28px (tab button) + ~2px (spacing) = 40px
    // Mobile: 8px (top padding) + ~28px (tab button with larger padding) = 36px
    tabsHeaderDesktop: 40,
    tabsHeaderMobile: 36,
} as const;

/**
 * Z-index hierarchy for page-level components
 */
export const Z_INDEX = {
    pageHeader: 95,
    gradientOverlay: 50,
} as const;
