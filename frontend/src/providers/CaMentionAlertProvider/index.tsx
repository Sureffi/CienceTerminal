/**
 * CaMentionAlerts Barrel Export
 *
 * Automatically exports demo or production provider based on VITE_DEMO_MODE
 */

import type { ReactNode } from 'react';
import { CaMentionAlertProvider as ProdProvider, useCaMentionAlerts as useProdAlerts } from './CaMentionAlertProvider';
import { CaMentionAlertProvider as DemoProvider, useCaMentionAlerts as useDemoAlerts } from './CaMentionAlertProvider.demo';

const isDemoMode = import.meta.env.VITE_DEMO_MODE === 'true';

// Export a wrapper provider that chooses the right implementation
export const CaMentionAlertProvider = ({ children }: { children: ReactNode }) => {
  const Provider = isDemoMode ? DemoProvider : ProdProvider;
  return <Provider>{children}</Provider>;
};

// Export a wrapper hook that uses the right implementation
export const useCaMentionAlerts = () => {
  if (isDemoMode) {
    return useDemoAlerts();
  } else {
    return useProdAlerts();
  }
};