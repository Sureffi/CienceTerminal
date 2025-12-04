/**
 * TwitterAlerts Barrel Export
 *
 * Automatically exports demo or production provider based on VITE_DEMO_MODE
 *
 * Note: TwitterAlertCard and TwitterAlertSection are old components
 * and are not exported here. Use TerminalPage for the new architecture.
 */

import type { ReactNode } from 'react';
import { TwitterAlertProvider as ProdProvider, useTwitterAlerts as useProdAlerts } from './TwitterAlertProvider';
import { TwitterAlertProvider as DemoProvider, useTwitterAlerts as useDemoAlerts } from './TwitterAlertProvider.demo';

const isDemoMode = import.meta.env.VITE_DEMO_MODE === 'true';

// Export a wrapper provider that chooses the right implementation
export const TwitterAlertProvider = ({ children }: { children: ReactNode }) => {
  const Provider = isDemoMode ? DemoProvider : ProdProvider;
  return <Provider>{children}</Provider>;
};

// Export a wrapper hook that uses the right implementation
export const useTwitterAlerts = () => {
  if (isDemoMode) {
    return useDemoAlerts();
  } else {
    return useProdAlerts();
  }
};