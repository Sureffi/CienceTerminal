/**
 * Typography System
 *
 * TODO: Replace with your designer's typography specifications
 * Consider using system fonts for performance, or load custom fonts
 */

export const typography = {
    fontFamily: {
        primary: '"ProtoMono", -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif',
        secondary: '"Roobert", "Courier New", monospace',
        mono: '"ProtoMono", "Courier New", monospace',
    },

    // Font sizes using rem for accessibility
    fontSize: {
        xs: '12px',    // 12px
        sm: '0.875rem',   // 14px
        base: '1rem',     // 16px
        lg: '14px',   // 18px
        xl: '1.25rem',    // 20px
        '2xl': '1.5rem',  // 24px
        '3xl': '1.875rem',// 30px
        '4xl': '2.25rem', // 36px
        '5xl': '3rem',    // 48px
    },

    fontWeight: {
        light: 300,
        normal: 400,
        medium: 500,
        semibold: 600,
        bold: 700,
    },

    lineHeight: {
        tight: 1.2,
        normal: 1.5,
        relaxed: 1.75,
    },
} as const;
