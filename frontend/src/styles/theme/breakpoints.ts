/**
 * Responsive Breakpoints
 *
 * Mobile-first approach
 */

export const breakpoints = {
  mobile: '640px',   // Small devices
  tablet: '768px',   // Tablets
  desktop: '1024px', // Laptops
  wide: '1280px',    // Desktops
} as const;

/**
 * Media query helpers for styled-components
 *
 * Usage:
 * ${({ theme }) => theme.media.tablet`
 *   font-size: 1.2rem;
 * `}
 */
export const media = {
  mobile: (styles: TemplateStringsArray | string) =>
    `@media (min-width: ${breakpoints.mobile}) { ${styles} }`,
  tablet: (styles: TemplateStringsArray | string) =>
    `@media (min-width: ${breakpoints.tablet}) { ${styles} }`,
  desktop: (styles: TemplateStringsArray | string) =>
    `@media (min-width: ${breakpoints.desktop}) { ${styles} }`,
  wide: (styles: TemplateStringsArray | string) =>
    `@media (min-width: ${breakpoints.wide}) { ${styles} }`,
} as const;
