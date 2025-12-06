import { useEffect, useState } from 'react';

/**
 * Custom hook for responsive media query detection
 *
 * @param query - Media query string (e.g., '(max-width: 768px)')
 * @returns boolean indicating if the media query matches
 *
 * @example
 * const isMobile = useMediaQuery('(max-width: 768px)');
 * const isTablet = useMediaQuery('(min-width: 769px) and (max-width: 1024px)');
 */
export const useMediaQuery = (query: string): boolean => {
    const [matches, setMatches] = useState<boolean>(false);

    useEffect(() => {
        // Create media query list
        const mediaQuery = window.matchMedia(query);

        // Set initial value
        setMatches(mediaQuery.matches);

        // Define listener
        const handleChange = (event: MediaQueryListEvent) => {
            setMatches(event.matches);
        };

        // Add listener for changes
        mediaQuery.addEventListener('change', handleChange);

        // Cleanup
        return () => {
            mediaQuery.removeEventListener('change', handleChange);
        };
    }, [query]);

    return matches;
};
