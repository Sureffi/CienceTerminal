/**
 * Shared constants for TokenTable and TokenRow components
 *
 * These constants ensure consistency between the table header and body rows.
 * Modifying grid columns here automatically updates both header and data rows.
 */

import { css } from 'styled-components';

/**
 * Grid column template - must match exactly between header and rows
 * Columns: TOKEN | 24H MENTIONS | AGE | PRICE CHANGE | MARKET CAP | VOLUME | LIQUIDITY | HOLDERS | TOP 10 HOLDERS
 */
export const GRID_COLUMNS = 'minmax(200px, 2fr) minmax(120px, 1.5fr) minmax(80px, 0.8fr) minmax(120px, 1.2fr) minmax(110px, 1.2fr) minmax(100px, 1fr) minmax(100px, 1fr) minmax(90px, 1fr) minmax(100px, 1fr)';

/**
 * Minimum table width to accommodate all columns
 */
export const MIN_TABLE_WIDTH = '1120px';

/**
 * Z-index layers for sticky positioning
 * Hierarchy: Sticky header intersection > Sticky column > Header > Body
 */
export const Z_INDEX = {
    body: 1,
    header: 20,
    stickyColumn: 40,
    stickyHeaderIntersection: 60,
} as const;

/**
 * Component dimensions for consistent sizing
 */
export const DIMENSIONS = {
    rowHeight: 44,
    tokenIconSize: 32,
    avatarSize: 28,
    actionButtonSize: 44,
    headerSpacerWidth: 50,
} as const;

/**
 * Shared sticky column styles with GPU acceleration
 * Apply to elements that need horizontal sticky positioning
 */
export const stickyColumnStyles = (zIndex: number) => css`
    position: sticky;
    left: 0;
    z-index: ${zIndex};
    will-change: transform;
    transform: translateZ(0);
    backface-visibility: hidden;
`;
