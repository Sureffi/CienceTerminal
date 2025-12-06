/**
 * Number and currency formatting utilities
 *
 * These functions provide consistent formatting across the application
 * for numbers, percentages, and currency values.
 */

/**
 * Format a number with K/M/B suffixes for readability
 *
 * @param num - The number to format (null/undefined returns '—')
 * @param isInteger - Whether to format as integer (no decimals for values < 1000)
 * @returns Formatted string like "1.5M", "456.7K", or "—"
 *
 * @example
 * formatNumber(1500000) // "1.5M"
 * formatNumber(450) // "450.00"
 * formatNumber(450, true) // "450"
 * formatNumber(null) // "—"
 */
export const formatNumber = (num: number | null | undefined, isInteger: boolean = false): string => {
    if (num == null || num === 0) return '—';
    if (num >= 1_000_000_000) return `${(num / 1_000_000_000).toFixed(1)}B`;
    if (num >= 1_000_000) return `${(num / 1_000_000).toFixed(1)}M`;
    if (num >= 1_000) return `${(num / 1_000).toFixed(1)}K`;
    return isInteger ? Math.floor(num).toString() : num.toFixed(2);
};

/**
 * Format a percentage value
 *
 * @param num - The percentage value (e.g., 5.23 for 5.23%)
 * @returns Formatted string like "5.2%" or "—"
 *
 * @example
 * formatPercent(5.234) // "5.2%"
 * formatPercent(null) // "—"
 */
export const formatPercent = (num: number | null | undefined): string => {
    if (num == null) return '—';
    return `${num.toFixed(1)}%`;
};

/**
 * Format a price change percentage with +/- sign
 *
 * @param num - The percentage change value
 * @returns Formatted string like "+5.23%" or "-2.15%"
 *
 * @example
 * formatPriceChange(5.234) // "+5.23%"
 * formatPriceChange(-2.156) // "-2.16%"
 * formatPriceChange(null) // "—"
 */
export const formatPriceChange = (num: number | null | undefined): string => {
    if (num == null) return '—';
    return `${num > 0 ? '+' : ''}${num.toFixed(2)}%`;
};
