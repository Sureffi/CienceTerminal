/**
 * Color Palette
 *
 * Semantic color naming for easy reference in styled-components
 * Usage: ${theme.colors.bgDark} or ${theme.colors.accentGreen}
 *
 * TODO: Replace these placeholder colors with your designer's brand colors
 */

export const colors = {
    // ========================================
    // Background Colors
    // ========================================
    bgDark: '#000000',              // Main background
    bgCard: '#111827',              // Card/container background
    bgElevated: '#1F2937',          // Elevated elements (modals, dropdowns)
    bgOverlay: 'rgba(0, 0, 0, 0.8)', // Modal overlay/backdrop

    // ========================================
    // Accent Colors (Brand)
    // ========================================
    accentGreen: '#13DE2E',         // Primary brand color (bright green)
    accentGreenLight: '#5CE1FF',    // Lighter variant
    accentGreenDark: '#00A8CC',     // Darker variant
    accentPurple: '#9D4EDD',        // Secondary accent
    accentPurpleLight: '#C77DFF',
    accentPurpleDark: '#7B2CBF',

    // ========================================
    // Text Colors
    // ========================================
    textPrimary: '#FFFFFF',         // Main text
    textGhost: 'rgba(255, 255, 255, 0.6)',       // Secondary text
    textMuted: 'rgba(255, 255, 255, 0.4)',           // Muted/tertiary text
    textDisabled: '#6B7280',        // Disabled text
    textInverse: '#111827',         // Text on light backgrounds
    textOnAccent: '#000000',        // Text on green accent backgrounds

    // ========================================
    // Border Colors
    // ========================================
    borderGreen: '#13DE2E',         // Accent borders
    borderDefault: 'rgba(255, 255, 255, 0.1)',       // Default borders
    borderSubtle: '#1F2937',        // Subtle borders
    borderGhost: "#161616",

    // ========================================
    // Status/Semantic Colors
    // ========================================
    statusSuccess: '#10B981',
    statusSuccessLight: '#34D399',
    statusSuccessDark: '#059669',

    statusWarning: '#F59E0B',
    statusWarningLight: '#FBBF24',
    statusWarningDark: '#D97706',

    statusError: '#EF4444',
    statusErrorLight: '#F87171',
    statusErrorDark: '#DC2626',

    statusInfo: '#3B82F6',
    statusInfoLight: '#60A5FA',
    statusInfoDark: '#2563EB',

    // ========================================
    // Neutral Grays (for granular control)
    // ========================================
    gray50: '#F9FAFB',
    gray100: '#F3F4F6',
    gray200: '#E5E7EB',
    gray300: '#D1D5DB',
    gray400: '#9CA3AF',
    gray500: '#6B7280',
    gray600: '#4B5563',
    gray700: '#374151',
    gray800: '#1F2937',
    gray900: '#111827',
} as const;
