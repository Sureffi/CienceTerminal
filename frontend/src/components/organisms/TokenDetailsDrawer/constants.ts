/**
 * Shared constants for TokenDetailsDrawer component
 *
 * These constants ensure consistency and maintainability for responsive design.
 * Following the same pattern as Header/constants.ts and TokenTable/constants.ts.
 */

/**
 * Responsive breakpoint for mobile devices
 * Matches Header, TokenTable, and Pages breakpoint for consistency
 */
export const MOBILE_BREAKPOINT = '768px';

/**
 * Component dimensions for consistent sizing across desktop and mobile
 */
export const DIMENSIONS = {
    drawer: {
        heightDesktop: '90vh',
        heightMobile: '90vh', // Slightly taller on mobile for better UX
        maxWidthDesktop: 1500,
        maxWidthContent: 1200,
        borderRadius: 16,
    },
    handle: {
        width: 48,
        height: 4,
        borderRadius: 2,
    },
    closeButton: {
        size: 32,
        iconSize: 14,
    },
    logo: {
        sizeDesktop: 40,
        sizeMobile: 32,
    },
    avatar: {
        sizeDesktop: 32,
        sizeMobile: 24,
    },
    icon: {
        followerIconDesktop: 9,
        followerIconMobile: 8,
    },
    button: {
        heightDesktop: 36,
        heightMobile: 32,
    },
    chart: {
        heightMobile: 350,
        heightTablet: 500,
        heightDesktop: 600,
    },
    card: {
        borderRadius: 6,
        buttonBorderRadius: 4,
    },
} as const;

/**
 * Spacing constants for padding and gaps
 */
export const SPACING = {
    padding: {
        drawerHeaderDesktop: '12px 16px',
        drawerHeaderMobile: '12px 16px',
        contentDesktop: '16px 24px',
        contentMobile: '12px 16px',
        headerSectionDesktop: '16px 24px',
        headerSectionMobile: '12px 16px',
        cardDesktop: '20px',
        cardMobile: '10px',
        message: '40px 20px',
        actionButtonDesktop: '10px 16px',
        actionButtonMobile: '8px 12px',
    },
    gap: {
        small: 4,
        medium: 8,
        mediumLarge: 12,
        large: 16,
        extraLarge: 24,
        huge: 48,
        cardDesktop: 16,
        cardMobile: 8,
        authorSectionDesktop: 8,
        authorSectionMobile: 6,
    },
    margin: {
        chartBottom: 16,
        mentionsSectionTop: 40,
        mentionsSectionBottom: 40,
        mentionsHeaderBottom: 24,
    },
} as const;

/**
 * Z-index hierarchy for drawer and overlay
 */
export const Z_INDEX = {
    overlay: 1000,
    content: 1001,
} as const;

/**
 * Typography settings for responsive font sizes
 */
export const TYPOGRAPHY = {
    tokenName: {
        sizeDesktop: 24,
        sizeMobile: 16,
        weight: 700,
    },
    metricLabel: {
        size: 11,
        weight: 500,
    },
    metricValue: {
        sizeDesktop: 20,
        sizeMobile: 12,
        weight: 600,
    },
    mentionsHeader: {
        size: 16,
        weight: 700,
    },
    authorUsername: {
        sizeDesktop: 14,
        sizeMobile: 12,
        weight: 600,
    },
    followerCount: {
        sizeDesktop: 12,
        sizeMobile: 11,
    },
    tweetText: {
        sizeDesktop: 14,
        sizeMobile: 11,
        lineHeight: 1.6,
        clampMobile: 3,
        clampTablet: 4,
        minHeightMobile: 67,
        minHeightTablet: 90,
    },
    actionButton: {
        sizeDesktop: 12,
        sizeMobile: 11,
        weight: 700,
    },
    externalButton: {
        sizeMobile: 12,
        sizeDesktop: 14,
        paddingMobile: '4px 10px',
        paddingDesktop: '5px 12px',
    },
    message: {
        size: 14,
    },
} as const;
