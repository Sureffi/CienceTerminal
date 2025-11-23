import { createGlobalStyle } from 'styled-components';

/**
 * Global Styles
 *
 * CSS reset and global styling using styled-components
 */
export const GlobalStyles = createGlobalStyle`
  /* CSS Reset */
  *, *::before, *::after {
    box-sizing: border-box;
    margin: 0;
    padding: 0;
  }

  /* Root element */
  html {
    font-size: 16px;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    text-rendering: optimizeLegibility;
  }

  /* Body */
  body {
    font-family: ${({ theme }) => theme.typography.fontFamily.primary};
    font-size: ${({ theme }) => theme.typography.fontSize.base};
    font-weight: ${({ theme }) => theme.typography.fontWeight.normal};
    line-height: ${({ theme }) => theme.typography.lineHeight.normal};
    color: ${({ theme }) => theme.colors.textPrimary};
    background-color: ${({ theme }) => theme.colors.bgDark};
    overflow-x: hidden;
  }

  /* Links */
  a {
    color: ${({ theme }) => theme.colors.accentGreen};
    text-decoration: none;
    transition: color ${({ theme }) => theme.transitions.fast};

    &:hover {
      color: ${({ theme }) => theme.colors.accentGreenLight};
    }

    &:focus-visible {
      outline: 2px solid ${({ theme }) => theme.colors.accentGreen};
      outline-offset: 2px;
    }
  }

  /* Buttons */
  button {
    font-family: inherit;
    cursor: pointer;
    border: none;
    background: none;

    &:disabled {
      cursor: not-allowed;
      opacity: 0.6;
    }

    &:focus-visible {
      outline: 2px solid ${({ theme }) => theme.colors.accentGreen};
      outline-offset: 2px;
    }
  }

  /* Inputs */
  input, textarea, select {
    font-family: inherit;
    font-size: inherit;

    &:focus-visible {
      outline: 2px solid ${({ theme }) => theme.colors.accentGreen};
      outline-offset: 2px;
    }
  }

  /* Remove input number spinners */
  input[type="number"]::-webkit-inner-spin-button,
  input[type="number"]::-webkit-outer-spin-button {
    -webkit-appearance: none;
    margin: 0;
  }

  input[type="number"] {
    -moz-appearance: textfield;
  }

  /* Headings */
  h1, h2, h3, h4, h5, h6 {
    font-weight: ${({ theme }) => theme.typography.fontWeight.bold};
    line-height: ${({ theme }) => theme.typography.lineHeight.tight};
    color: ${({ theme }) => theme.colors.textPrimary};
  }

  h1 {
    font-size: ${({ theme }) => theme.typography.fontSize['4xl']};
  }

  h2 {
    font-size: ${({ theme }) => theme.typography.fontSize['3xl']};
  }

  h3 {
    font-size: ${({ theme }) => theme.typography.fontSize['2xl']};
  }

  h4 {
    font-size: ${({ theme }) => theme.typography.fontSize.xl};
  }

  h5 {
    font-size: ${({ theme }) => theme.typography.fontSize.lg};
  }

  h6 {
    font-size: ${({ theme }) => theme.typography.fontSize.base};
  }

  /* Lists */
  ul, ol {
    list-style: none;
  }

  /* Images */
  img {
    max-width: 100%;
    height: auto;
    display: block;
  }

  /* Code blocks */
  code, pre {
    font-family: ${({ theme }) => theme.typography.fontFamily.mono};
    font-size: ${({ theme }) => theme.typography.fontSize.sm};
  }

  /* Scrollbar styling (webkit browsers) */
  ::-webkit-scrollbar {
    width: 8px;
    height: 8px;
  }

  ::-webkit-scrollbar-track {
    background: ${({ theme }) => theme.colors.bgCard};
  }

  ::-webkit-scrollbar-thumb {
    background: ${({ theme }) => theme.colors.gray600};
    border-radius: ${({ theme }) => theme.borderRadius.md};

    &:hover {
      background: ${({ theme }) => theme.colors.gray500};
    }
  }

  /* Selection */
  ::selection {
    background-color: ${({ theme }) => theme.colors.accentGreen};
    color: ${({ theme }) => theme.colors.textOnAccent};
  }

  /* Focus visible (for keyboard navigation) */
  *:focus-visible {
    outline: 2px solid ${({ theme }) => theme.colors.accentGreen};
    outline-offset: 2px;
  }

  /* Accessibility - Skip to main content link */
  .skip-to-main {
    position: absolute;
    top: -100%;
    left: 0;
    padding: ${({ theme }) => theme.spacing.md};
    background: ${({ theme }) => theme.colors.accentGreen};
    color: ${({ theme }) => theme.colors.textOnAccent};
    z-index: ${({ theme }) => theme.zIndex.tooltip};

    &:focus {
      top: 0;
    }
  }

  /* Print styles */
  @media print {
    body {
      background: white;
      color: black;
    }
  }
`;
