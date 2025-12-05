import 'styled-components';

// Extend styled-components DefaultTheme with our custom theme
declare module 'styled-components' {
    export interface DefaultTheme {
        colors: {
            // Background colors
            bgDark: string;
            bgCard: string;
            bgElevated: string;
            bgOverlay: string;

            // Accent colors
            accentGreen: string;
            accentGreenLight: string;
            accentGreenDark: string;
            accentPurple: string;
            accentPurpleLight: string;
            accentPurpleDark: string;

            // Text colors
            textPrimary: string;
            textSecondary: string;
            textGhost: string;
            textMuted: string;
            textDisabled: string;
            textInverse: string;
            textOnAccent: string;

            // Border colors
            borderGreen: string;
            borderDefault: string;
            borderSubtle: string;
            borderGhost: string;

            // Status colors
            statusSuccess: string;
            statusSuccessLight: string;
            statusSuccessDark: string;
            statusWarning: string;
            statusWarningLight: string;
            statusWarningDark: string;
            statusError: string;
            statusErrorLight: string;
            statusErrorDark: string;
            statusInfo: string;
            statusInfoLight: string;
            statusInfoDark: string;

            // Neutral grays
            gray50: string;
            gray100: string;
            gray200: string;
            gray300: string;
            gray400: string;
            gray500: string;
            gray600: string;
            gray700: string;
            gray800: string;
            gray900: string;
        };
        typography: {
            fontFamily: {
                primary: string;
                secondary: string;
                mono: string;
            };
            fontSize: {
                xs: string;
                sm: string;
                base: string;
                lg: string;
                xl: string;
                '2xl': string;
                '3xl': string;
                '4xl': string;
                '5xl': string;
            };
            fontWeight: {
                light: number;
                normal: number;
                medium: number;
                semibold: number;
                bold: number;
            };
            lineHeight: {
                tight: number;
                normal: number;
                relaxed: number;
            };
        };
        spacing: {
            xs: string;
            sm: string;
            md: string;
            lg: string;
            xl: string;
            '2xl': string;
            '3xl': string;
            '4xl': string;
            '5xl': string;
        };
        breakpoints: {
            mobile: string;
            tablet: string;
            desktop: string;
            wide: string;
        };
        media: {
            mobile: (styles: TemplateStringsArray | string) => string;
            tablet: (styles: TemplateStringsArray | string) => string;
            desktop: (styles: TemplateStringsArray | string) => string;
            wide: (styles: TemplateStringsArray | string) => string;
        };
        shadows: {
            none: string;
            sm: string;
            md: string;
            lg: string;
            xl: string;
        };
        transitions: {
            fast: string;
            normal: string;
            slow: string;
        };
        borderRadius: {
            none: string;
            sm: string;
            md: string;
            lg: string;
            xl: string;
            full: string;
        };
        zIndex: {
            base: number;
            dropdown: number;
            sticky: number;
            fixed: number;
            modal: number;
            popover: number;
            tooltip: number;
        };
    }
}
