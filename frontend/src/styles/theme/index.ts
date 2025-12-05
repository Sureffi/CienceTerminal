import type { DefaultTheme } from 'styled-components';
import { colors } from './colors';
import { typography } from './typography';
import { spacing } from './spacing';
import { breakpoints, media } from './breakpoints';
import { shadows } from './shadows';

/**
 * Main Theme Object
 *
 * This is the centralized theme that will be accessible to all styled-components
 * via the ThemeProvider and the ${({ theme }) => theme.xxx} syntax
 */
export const theme: DefaultTheme = {
    colors,
    typography,
    spacing,
    breakpoints,
    media,
    shadows,

    // Transitions
    transitions: {
        fast: '150ms ease-in-out',
        normal: '250ms ease-in-out',
        slow: '350ms ease-in-out',
    },

    // Border radius
    borderRadius: {
        none: '0',
        sm: '0.25rem',   // 4px
        md: '3px',    // 8px
        lg: '0.75rem',   // 12px
        xl: '1rem',      // 16px
        full: '9999px',  // Pill shape
    },

    // Z-index scale
    zIndex: {
        base: 0,
        dropdown: 1000,
        sticky: 1100,
        fixed: 1200,
        modal: 1300,
        popover: 1400,
        tooltip: 1500,
    },
};

// Export individual pieces for convenience
export * from './colors';
export * from './typography';
export * from './spacing';
export * from './breakpoints';
export * from './shadows';
