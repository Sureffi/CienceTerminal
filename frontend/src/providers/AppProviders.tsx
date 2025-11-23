import type { ReactNode } from 'react';
import { TwitterAlertProvider } from './TwitterAlertProvider';
import { CaMentionAlertProvider } from './CaMentionAlertProvider';

/**
 * AppProviders
 *
 * Composition of all global providers for the application.
 * This provides a clean separation of concerns and makes it easy to:
 * - Add/remove providers in one place
 * - See the provider hierarchy at a glance
 * - Toggle between demo/production modes via config
 *
 * Provider order matters - inner providers can access outer provider contexts.
 */

interface AppProvidersProps {
    children: ReactNode;
}

export const AppProviders = ({ children }: AppProvidersProps) => {
    return (
        <TwitterAlertProvider>
            <CaMentionAlertProvider>
                {children}
            </CaMentionAlertProvider>
        </TwitterAlertProvider>
    );
};
