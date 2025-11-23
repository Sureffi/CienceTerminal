/**
 * Environment Configuration
 *
 * Central configuration for demo mode and other environment settings.
 *
 * To toggle demo mode: Change isDemoMode to true/false
 * - Demo mode: Uses mock data, no authentication, no backend connection
 * - Production mode: Real backend, SignalR, authentication required
 */

export const config = {
    /**
     * Demo Mode Toggle
     *
     * true  = Demo mode (mock providers, no auth, no backend)
     * false = Production mode (real providers, auth, backend connection)
     */
    isDemoMode: false,

    /**
     * Derived: Whether authentication is required
     */
    get requiresAuth() {
        return !this.isDemoMode;
    },
} as const;
